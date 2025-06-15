using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ProphetPlay
{
    public partial class MainWindow : Window
    {
        public string AktuellerBenutzername { get; set; }
        public string AktuelleRolle { get; set; }
        public ObservableCollection<LoginResponse> BenutzerListe { get; set; } = new();

        // alle Ligen, gefilterte Ligen
        private List<LeaguesArticle> alleLigen = new();
        private ObservableCollection<LeaguesArticle> gefilterteLigen = new();

        public MainWindow(string benutzername, string rolle)
        {
            InitializeComponent();

            AktuellerBenutzername = benutzername;
            AktuelleRolle = rolle;
            DataContext = this;

            LoadNews();
            _ = LoadLeaguesAsync();

            AdminPanel.Visibility = (AktuelleRolle == "Admin")
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (AktuelleRolle == "Admin")
                _ = LadeBenutzerListeAsync(AktuellerBenutzername);
        }

        private async void LoadNews()
        {
            try
            {
                var news = await NewsService.GetFootballNewsAsync();
                NewsListBox.ItemsSource = news;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Nachrichten: {ex.Message}");
            }
        }

        private async Task LoadLeaguesAsync()
        {
            try
            {
                alleLigen = await ApiFootballService.GetLeaguesAsync();
                gefilterteLigen = new ObservableCollection<LeaguesArticle>(alleLigen);
                ListBoxLeaguen.ItemsSource = gefilterteLigen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Ligen: {ex.Message}");
            }
        }

        private async Task LadeBenutzerListeAsync(string requester)
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
            var response = await client.GetAsync($"/api/benutzer/liste?requester={requester}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = System.Text.Json.JsonSerializer
                              .Deserialize<List<LoginResponse>>(json);
                BenutzerListe.Clear();
                foreach (var u in users) BenutzerListe.Add(u);
            }
            else
            {
                MessageBox.Show($"Fehler beim Laden der Benutzer: {response.StatusCode}");
            }
        }

        private void NewsListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (NewsListBox.SelectedItem is NewsArticle news)
            {
                MessageBox.Show(
                    $"{news.Title}\n\n{news.Description}\n\nVeröffentlicht: {news.PublishedAt:dd.MM.yyyy HH:mm}\n\n{news.Url}",
                    "News-Details",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void TextBoxLeaguen_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxLeaguen.Text == "🔍 search ...")
            {
                TextBoxLeaguen.Text = "";
                TextBoxLeaguen.Foreground = Brushes.Black;
            }
        }

        private void TextBoxLeaguen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLeaguen.Text))
            {
                TextBoxLeaguen.Text = "🔍 search ...";
                TextBoxLeaguen.Foreground = Brushes.Gray;
                // alle Ligen wiederherstellen
                gefilterteLigen.Clear();
                foreach (var l in alleLigen) gefilterteLigen.Add(l);
            }
        }

        private void TextBoxLeaguen_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Placeholder ignorieren
            if (TextBoxLeaguen.Foreground == Brushes.Gray) return;

            var query = TextBoxLeaguen.Text.Trim();
            gefilterteLigen.Clear();
            foreach (var liga in alleLigen
                         .Where(l => l.LeagueName.Contains(query, StringComparison.OrdinalIgnoreCase)
                                  || l.CountryName.Contains(query, StringComparison.OrdinalIgnoreCase)))
            {
                gefilterteLigen.Add(liga);
            }
        }

        private void LeagueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LeaguesArticle league)
            {
                var fenster = new SpieleFenster(league);
                fenster.Show();
            }
        }

        private void Spiele_anzeigen_Button(object sender, RoutedEventArgs e)
        {
            // Deine bestehende Logik hier…
        }

        private async void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is string targetBenutzer))
                return;

            var confirm = MessageBox.Show(
                $"Willst du '{targetBenutzer}' wirklich löschen?",
                "Löschen bestätigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
                // Baue deine DELETE-Request manuell, um Header setzen zu können
                var request = new HttpRequestMessage(HttpMethod.Delete,
                    $"/api/benutzer/loeschen?requester={AktuellerBenutzername}&target={targetBenutzer}"
                );
                request.Headers.Add("Prefer", "return=minimal");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Benutzer erfolgreich gelöscht.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LadeBenutzerListeAsync(AktuellerBenutzername);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Nicht autorisiert.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Fehler: {response.StatusCode}\n{err}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ausnahme beim Löschen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
