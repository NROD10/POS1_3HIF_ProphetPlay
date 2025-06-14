using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;



namespace ProphetPlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AktuellerBenutzername { get; set; }
        public string AktuelleRolle { get; set; }

        public ObservableCollection<LoginResponse> BenutzerListe { get; set; } = new();

        public MainWindow(string benutzername, string rolle)
        {
            InitializeComponent();
            AktuellerBenutzername = benutzername;
            AktuelleRolle = rolle;

            this.DataContext = this;

            LoadNews();
            LoadLeagues();

            if (AktuelleRolle == "Admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
                _ = LadeBenutzerListeAsync(AktuellerBenutzername);
            }
            else
            {
                AdminPanel.Visibility = Visibility.Collapsed;
            }

        }


        public async Task LadeBenutzerListeAsync(string requester)
        {
            using HttpClient client = new();
            client.BaseAddress = new Uri("http://localhost:8080");

            var response = await client.GetAsync($"/api/benutzer/liste?requester={requester}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<LoginResponse>>(json);
                BenutzerListe.Clear();
                foreach (var user in users)
                {
                    BenutzerListe.Add(user);
                }
            }
            else
            {
                MessageBox.Show($"Fehler beim Laden: {response.StatusCode}");
            }
        }

        private async void LoeschenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string targetBenutzer)
            {
                string requester = this.AktuellerBenutzername; // Eigene Property oder Login-Session

                using HttpClient client = new();
                client.BaseAddress = new Uri("http://localhost:8080");

                var response = await client.DeleteAsync($"/api/benutzer/loeschen?requester={requester}&target={targetBenutzer}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Benutzer gelöscht");
                    await LadeBenutzerListeAsync(requester); // Liste neu laden
                }
                else
                {
                    MessageBox.Show($"Fehler beim Löschen: {response.StatusCode}");
                }
            }
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

        private void NewsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (NewsListBox.SelectedItem is NewsArticle news)
            {
                string message = $"{news.Title}\n\n{news.Description}\n\nVeröffentlicht: {news.PublishedAt:dd.MM.yyyy HH:mm}\n\n{news.Url}";
                MessageBox.Show(message, "News-Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void LoadLeagues()
        {
            try
            {
                var leagues = await ApiFootballService.GetLeaguesAsync();
                ListBoxLeaguen.ItemsSource = leagues;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Leaguen: {ex.Message}");
            }
        }

        private void LeagueButton_Click(object sender, RoutedEventArgs e)
        {
            // Prompt: wie mach ich das, dass das neu erstellte Fenster weiß welche Liga das ist?
            Button btn = sender as Button;
            if (btn != null)
            {
                LeaguesArticle league = btn.DataContext as LeaguesArticle;
                if (league != null)
                {
                    SpieleFenster fenster = new SpieleFenster(league);
                    fenster.Show();
                }
            }

        }


        // Prompt: wie mach ich das das suchen vom Texfeld verschwindet sobald man auf das Textfeld draufklickt, gibts dafür ein Befehl im xaml?
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
            if (TextBoxLeaguen.Text == "")
            {
                TextBoxLeaguen.Text = "🔍 search ...";
                TextBoxLeaguen.Foreground = Brushes.Gray;
            }
        }


        private List<LeaguesArticle> alleLigen;
        private void Spiele_anzeigen_Button(object sender, RoutedEventArgs e)
        {


            string eingabe = TextBoxLeaguen.Text;

            var liga = alleLigen?.FirstOrDefault(l =>l.LeagueName.Equals(eingabe, StringComparison.OrdinalIgnoreCase));


            
            if (liga == null)
            {
                SpieleFenster fenster = new SpieleFenster(liga);
                fenster.Show();
            }

        }
    }

    
}