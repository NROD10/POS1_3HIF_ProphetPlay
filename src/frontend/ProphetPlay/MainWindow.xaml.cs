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
                // Wieder alle anzeigen
                gefilterteLigen.Clear();
                foreach (var l in alleLigen) gefilterteLigen.Add(l);
            }
        }

        private void TextBoxLeaguen_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Wenn Placeholder, nichts tun
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
            // bleibt unverändert oder kann ähnliche Logik nutzen...
        }

        private async void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string targetBenutzer)
            {
                string requester = this.AktuellerBenutzername;
                using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };

                try
                {
                    var response = await client.DeleteAsync(
                        $"/api/benutzer/loeschen?requester={requester}&target={targetBenutzer}"
                    );

                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("✅ Benutzer gelöscht");
                        await LadeBenutzerListeAsync(requester);
                    }
                    else
                    {
                        MessageBox.Show($"❌ Fehler: {response.StatusCode}\n{content}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }



    }
}
