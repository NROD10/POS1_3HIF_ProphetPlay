using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProphetPlay
{
    public partial class SpieleFenster : Window
    {
        // Die ausgewählte Liga, z.B. Bundesliga
        private readonly LeaguesArticle _league;

        public SpieleFenster(LeaguesArticle league)
        {
            InitializeComponent();
            _league = league;

            // Überschriften und Logo anzeigen
            ligen_ueberschrift_textblock.Text = league.LeagueName;
            land_ueberschrift_textblock.Text = league.CountryName;
            logo_image.Source = new BitmapImage(new Uri(league.LogoUrl));

            // Sobald Fenster fertig geladen ist → Spiele laden
            Loaded += async (_, __) => await LoadGamesAsync();
        }

        // Damit Scrollen mit der Maus auch funktioniert (z.B. wenn ScrollViewer benutzt wird)
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                // Scrollen nach oben/unten je nach Mausrad
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        // Lädt vergangene und zukünftige Spiele
        private async Task LoadGamesAsync()
        {
            // 1) Letzte 5 vergangene Spiele der Liga abrufen
            var past = await ApiFootballService.GetPastMatchesAsync(_league.LeagueId, _league.Season, last: 5);

            // 2) Nächste 10 Spiele holen
            var upcoming = await ApiFootballService.GetUpcomingMatchesAsync(_league.LeagueId, _league.Season, next: 10);

            // Falls keine kommenden Spiele → Alternative: Spiele in den nächsten 30 Tagen
            if (upcoming == null || !upcoming.Any())
            {
                upcoming = await ApiFootballService.GetMatchesByDateRangeAsync(_league.LeagueId, _league.Season, days: 30);
            }

            // 3) Daten anzeigen in den ListBoxen
            ListBoxPastSpiele.ItemsSource = past;
            ListBoxLiveSpiele.ItemsSource = upcoming;

            // Wenn keine Spiele vorhanden sind → Hinweis anzeigen
            KeineSpieleTextBlock.Visibility = (past.Any() || upcoming.Any())
                                              ? Visibility.Collapsed
                                              : Visibility.Visible;
        }

        // Wenn auf ein Spiel doppelt geklickt wird (egal ob vergangen oder live)
        private void ListBoxGame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as System.Windows.Controls.ListBox)?.SelectedItem is LiveMatchResponse match)
            {
                // Neues Detailfenster für dieses Spiel öffnen
                var detailFenster = new SpielDetailsWindow(match);
                detailFenster.Show();
                // jaja
            }
        }
    }
}
