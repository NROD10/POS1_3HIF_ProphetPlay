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
    // <summary>
    /// Hauptfenster der Anwendung
    /// Zeigt Ligen, News und Admin-Funktionen an
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Der aktuell angemeldete Benutzername
        /// </summary>
        public string AktuellerBenutzername { get; set; }

        /// <summary>
        /// Die Rolle des aktuellen Benutzers entweder Admin oder User
        /// </summary>
        public string AktuelleRolle { get; set; }

        /// <summary>
        /// Liste aller Benutzer die nur für Admin sichtbar sind
        /// </summary>
        public ObservableCollection<LoginResponse> BenutzerListe { get; set; } = new();

        /// <summary>
        /// Liste aller verfügbaren Ligen
        /// </summary>
        private List<LeaguesArticle> alleLigen = new();

        /// <summary>
        /// Gefilterte Ligen nach Suchbegriff
        /// </summary>
        private ObservableCollection<LeaguesArticle> gefilterteLigen = new();

        /// <summary>
        /// Initialisiert das Hauptfenster und lädt Daten
        /// </summary>
        /// <param name="benutzername">Angemeldeter Benutzername</param>
        /// <param name="rolle">Benutzerrolle ("Admin" oder "User")</param>
        public MainWindow(string benutzername, string rolle)
        {
            InitializeComponent();
            AktuellerBenutzername = benutzername;
            AktuelleRolle = rolle;
            DataContext = this;

            LoggerService.Logger.Information("MainWindow gestartet für Benutzer: {0}, Rolle: {1}", benutzername, rolle);

            _ = LoadNewsAsync();
            _ = LoadLeaguesAsync();

            // Admin-Panel nur für Admins sichtbar
            LoggerService.Logger.Information("AdminPanel aktiviert für Benutzer: {0}", benutzername);
            AdminPanel.Visibility = AktuelleRolle == "Admin"
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Benutzerliste nur für Admin laden
            if (AktuelleRolle == "Admin")
                _ = LadeBenutzerListeAsync(AktuellerBenutzername);
        }

        /// <summary>
        /// Lädt interne und externe Fußball-News und zeigt sie in der NewsListBox an
        /// </summary>
        /// 
        // Prompt: "LoadNewsAsync: kombiniere interne News und externe Fußball-News und zeig sie in der ListBox an"
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
                LoggerService.Logger.Error(ex, "Fehler beim Laden der Nachrichten");
                MessageBox.Show($"Fehler beim Laden der Nachrichten: {ex.Message}", "Fehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lädt die Liste aller verfügbaren Ligen von der API und zeigt sie im UI an
        /// </summary>
        /// 
        // Prompt: "LoadLeaguesAsync: ruf Ligen von ApiFootballService ab, speicher in alleLigen und gefilterteLigen und bind ListBox"

        private async Task LoadLeaguesAsync()
        {
            try
            {
                LoggerService.Logger.Information("Lade Ligen von API für Benutzer: {0}", AktuellerBenutzername);
                alleLigen = await ApiFootballService.GetLeaguesAsync();
                gefilterteLigen = new ObservableCollection<LeaguesArticle>(alleLigen);
                ListBoxLeaguen.ItemsSource = gefilterteLigen;
                LoggerService.Logger.Information("Ligen erfolgreich geladen: {0} Einträge", alleLigen.Count);
            }
            catch (Exception ex)
            {
                LoggerService.Logger.Error(ex, "Fehler beim Laden der Ligen");
                MessageBox.Show($"Fehler beim Laden der Ligen: {ex.Message}", "Fehler",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lädt die Benutzerliste vom Server die nur für Admins sichtbar ist
        /// </summary>
        /// <param name="requester">Der Benutzername des aktuellen Admins</param>
        /// 
        // Prompt: "LadeBenutzerListeAsync: hol alle Benutzer vom Backend, nur Admins dürfen das
        private async Task LadeBenutzerListeAsync(string requester)
        {
            LoggerService.Logger.Information("Lade Benutzerliste für Admin: {0}", requester);
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
            var resp = await client.GetAsync($"/api/benutzer/liste?requester={requester}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var users = System.Text.Json.JsonSerializer.Deserialize<List<LoginResponse>>(json);
                BenutzerListe.Clear();
                foreach (var u in users)
                {
                    BenutzerListe.Add(u);
                }
                LoggerService.Logger.Information("Benutzerliste geladen mit {0} Einträgen", BenutzerListe.Count);
            }
            else
            {
                LoggerService.Logger.Warning("Fehler beim Abrufen der Benutzerliste: {0}", resp.StatusCode);
            }
        }

        /// <summary>
        /// Zeigt Details der ausgewählten News in einem Dialog an
        /// </summary>
        private void NewsListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (NewsListBox.SelectedItem is NewsArticle news)
            {
                LoggerService.Logger.Information("News ausgewählt: {Title}", news.Title);

                MessageBox.Show(
                    $"{news.Title}\n\n{news.Description}\n\nVeröffentlicht: {news.PublishedAt:dd.MM.yyyy HH:mm}\n\n{news.Url}",
                    "News-Details", MessageBoxButton.OK, MessageBoxImage.Information
                );
            }
        }


        /// <summary>
        /// Wird ausgelöst, wenn das Suchfeld für Ligen den Fokus erhält
        /// Setzt den Platzhaltertext zurück und ändert die Textfarbe
        /// </summary>
        private void TextBoxLeaguen_GotFocus(object sender, RoutedEventArgs e)
        {
            LoggerService.Logger.Information("Suchfeld für Ligen fokussiert");

            if (TextBoxLeaguen.Text == "🔍 search ...")
            {
                TextBoxLeaguen.Text = "";
                TextBoxLeaguen.Foreground = System.Windows.Media.Brushes.Black;
            }
        }


        /// <summary>
        /// Wird ausgelöst, wenn das Suchfeld für Ligen den Fokus verliert
        /// Setzt den Platzhaltertext zurück und zeigt alle Ligen an falls das Feld leer ist
        /// </summary>
        private void TextBoxLeaguen_LostFocus(object sender, RoutedEventArgs e)
        {
            LoggerService.Logger.Information("Suchfeld für Ligen Fokus verloren");

            if (string.IsNullOrWhiteSpace(TextBoxLeaguen.Text))
            {
                TextBoxLeaguen.Text = "🔍 search ...";
                TextBoxLeaguen.Foreground = System.Windows.Media.Brushes.Gray;
                gefilterteLigen.Clear();
                foreach (var l in alleLigen) gefilterteLigen.Add(l);
            }
        }


        /// <summary>
        /// Wird ausgelöst, wenn sich der Text im Suchfeld für Ligen ändert
        /// Filtert die Ligenliste nach dem eingegebenen Suchbegriff
        /// </summary>
        private void TextBoxLeaguen_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxLeaguen.Foreground == System.Windows.Media.Brushes.Gray) return;

            var q = TextBoxLeaguen.Text.Trim();
            LoggerService.Logger.Information("Liga-Suche geändert: {0}", q);

            gefilterteLigen.Clear();
            foreach (var l in alleLigen
                .Where(lg => lg.LeagueName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                             lg.CountryName.Contains(q, StringComparison.OrdinalIgnoreCase)))
            {
                gefilterteLigen.Add(l);
            }
        }


        /// <summary>
        /// Öffnet das Spielefenster für die ausgewählte Liga
        /// </summary>
        private void LeagueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LeaguesArticle league)
            {
                LoggerService.Logger.Information("Liga ausgewählt: {0}", league.LeagueName);
                new SpieleFenster(league).Show();
            }
        }

        /// <summary>
        /// Löscht einen Admin-Benutzer nach Bestätigung
        /// </summary>
        /// 

        // Prompt: "LoeschenButton_Click: lösche Admin-Benutzer nach Bestätigung
        private async void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string target)
            {
                if (MessageBox.Show($"Willst du '{target}' wirklich löschen?", "Löschen bestätigen",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

                LoggerService.Logger.Information("Admin-Benutzer löschen: {0}", target);

                using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
                var req = new HttpRequestMessage(HttpMethod.Delete,
                    $"/api/benutzer/loeschen?requester={AktuellerBenutzername}&target={target}");
                req.Headers.Add("Prefer", "return=minimal");
                var resp = await client.SendAsync(req);

                if (resp.IsSuccessStatusCode)
                {
                    LoggerService.Logger.Information("Benutzer {0} erfolgreich gelöscht", target);
                    await LadeBenutzerListeAsync(AktuellerBenutzername);
                }
                else
                {
                    LoggerService.Logger.Error("Fehler beim Löschen von Benutzer {0}: {1}", target, resp.StatusCode);
                    MessageBox.Show($"Fehler: {resp.StatusCode}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Löscht eine News nach Bestätigung
        /// </summary>
        private async void NewsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
            {
                if (MessageBox.Show("Möchtest du diese News wirklich löschen?", "News löschen",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

                LoggerService.Logger.Information("News löschen: ID {0}", id);

                var ok = await NewsService.DeleteNewsAsync(id, AktuellerBenutzername);
                if (ok)
                {
                    LoggerService.Logger.Information("News ID {0} erfolgreich gelöscht", id);
                    await LoadNewsAsync();
                }
                else
                {
                    LoggerService.Logger.Error("Fehler beim Löschen der News ID {0}", id);
                    MessageBox.Show("Fehler beim Löschen der News.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Meldet den Benutzer nach Bestätigung ab
        /// </summary>
        private void AbmeldenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Willst du dich wirklich abmelden?", "Abmelden",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LoggerService.Logger.Information("Benutzer {0} meldet sich ab", AktuellerBenutzername);
                new LoginWindow().Show();
                Close();
            }
        }

        /// <summary>
        /// Öffnet das Fenster zur Erstellung einer neuen News und lädt danach die News neu
        /// </summary>
        private async void BtnCreateNews_Click(object sender, RoutedEventArgs e)
        {
            var win = new CreateNewsWindow(AktuellerBenutzername)
            {
                Owner = this
            };
            if (win.ShowDialog() == true)
            {
                LoggerService.Logger.Information("Neue News erstellt von Benutzer {0}", AktuellerBenutzername);
                await LoadNewsAsync();
            }
        }

    }
}
