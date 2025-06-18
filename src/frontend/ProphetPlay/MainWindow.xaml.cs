using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

            _ = LoadNewsAsync();
            _ = LoadLeaguesAsync();

            AdminPanel.Visibility = AktuelleRolle == "Admin"
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (AktuelleRolle == "Admin")
                _ = LadeBenutzerListeAsync(AktuellerBenutzername);
        }

        private async Task LoadNewsAsync()
        {
            try
            {
                var combined = new List<NewsArticle>();
                try { combined.AddRange(await NewsService.GetAllNewsAsync()); }
                catch { /* ignore */ }
                try { combined.AddRange(await NewsService.GetFootballNewsAsync()); }
                catch (Exception ex) { Debug.WriteLine("Fehler externe News: " + ex.Message); }
                NewsListBox.ItemsSource = combined;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Nachrichten: {ex.Message}", "Fehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Fehler beim Laden der Ligen: {ex.Message}", "Fehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LadeBenutzerListeAsync(string requester)
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
            var resp = await client.GetAsync($"/api/benutzer/liste?requester={requester}");
            if (resp.IsSuccessStatusCode)
            {
                var users = Newtonsoft.Json.JsonConvert
                             .DeserializeObject<List<LoginResponse>>(await resp.Content.ReadAsStringAsync());
                BenutzerListe.Clear();
                foreach (var u in users) BenutzerListe.Add(u);
            }
        }

        private void NewsListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (NewsListBox.SelectedItem is NewsArticle news)
            {
                MessageBox.Show(
                    $"{news.Title}\n\n{news.Description}\n\nVeröffentlicht: {news.PublishedAt:dd.MM.yyyy HH:mm}\n\n{news.Url}",
                    "News-Details", MessageBoxButton.OK, MessageBoxImage.Information
                );
            }
        }

        private void TextBoxLeaguen_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxLeaguen.Text == "🔍 search ...")
            {
                TextBoxLeaguen.Text = "";
                TextBoxLeaguen.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBoxLeaguen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLeaguen.Text))
            {
                TextBoxLeaguen.Text = "🔍 search ...";
                TextBoxLeaguen.Foreground = System.Windows.Media.Brushes.Gray;
                gefilterteLigen.Clear();
                foreach (var l in alleLigen) gefilterteLigen.Add(l);
            }
        }

        private void TextBoxLeaguen_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxLeaguen.Foreground == System.Windows.Media.Brushes.Gray) return;
            var q = TextBoxLeaguen.Text.Trim();
            gefilterteLigen.Clear();
            foreach (var l in alleLigen
                .Where(lg => lg.LeagueName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                             lg.CountryName.Contains(q, StringComparison.OrdinalIgnoreCase)))
            {
                gefilterteLigen.Add(l);
            }
        }

        private void LeagueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LeaguesArticle league)
                new SpieleFenster(league).Show();
        }

        private async void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            // Admin-Benutzer löschen
            if (sender is Button btn && btn.Tag is string target)
            {
                if (MessageBox.Show($"Willst du '{target}' wirklich löschen?", "Löschen bestätigen",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

                using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
                var req = new HttpRequestMessage(HttpMethod.Delete,
                    $"/api/benutzer/loeschen?requester={AktuellerBenutzername}&target={target}");
                req.Headers.Add("Prefer", "return=minimal");
                var resp = await client.SendAsync(req);
                if (resp.IsSuccessStatusCode) await LadeBenutzerListeAsync(AktuellerBenutzername);
                else MessageBox.Show($"Fehler: {resp.StatusCode}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void NewsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
            {
                if (MessageBox.Show("Möchtest du diese News wirklich löschen?", "News löschen",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

                var ok = await NewsService.DeleteNewsAsync(id, AktuellerBenutzername);
                if (ok) await LoadNewsAsync();
                else MessageBox.Show("Fehler beim Löschen der News.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AbmeldenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Willst du dich wirklich abmelden?", "Abmelden",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                Close();
            }
        }

        private async void BtnCreateNews_Click(object sender, RoutedEventArgs e)
        {
            // übergebe den aktuellen Benutzernamen
            var win = new CreateNewsWindow(AktuellerBenutzername)
            {
                Owner = this
            };
            if (win.ShowDialog() == true)
                await LoadNewsAsync();
        }

    }
}
