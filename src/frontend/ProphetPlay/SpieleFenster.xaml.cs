using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace ProphetPlay
{
    /// <summary>
    /// Fenster zur Anzeige von Spielen einer bestimmten Liga.
    /// Zeigt vergangene und kommende Spiele sowie Ligadetails wie Name, Land und Logo.
    /// </summary>
    public partial class SpieleFenster : Window
    {

        /// <summary>
        /// Die aktuell ausgewählte Liga
        /// </summary>
        private readonly LeaguesArticle _league;

        /// <summary>
        /// Erstellt ein neues Fenster zur Anzeige von Spielen der Liga
        /// </summary>
        /// <param name="league">Die Liga deren Spiele angezeigt werden sollen</param>
        /// 

        // prompt: "bau constructor so, dass Header, Land und Logo angezeigt werden und dann automatisch Spiele geladen werden"
        public SpieleFenster(LeaguesArticle league)
        {
            InitializeComponent();
            _league = league;

            LoggerService.Logger.Information("SpieleFenster geöffnet für Liga: {0} ({1})", league.LeagueName, league.LeagueId);

            /// <summary>
            /// UI mit Ligadetails füllen
            /// </summary>
            ligen_ueberschrift_textblock.Text = league.LeagueName;
            land_ueberschrift_textblock.Text = league.CountryName;
            logo_image.Source = new BitmapImage(new Uri(league.LogoUrl));

            this.Loaded += async (sender, args) =>
            {
                await LoadGamesAsync();
            };
        }

        /// <summary>
        /// Ermöglicht vertikales Scrollen mit dem Mausrad
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Lädt vergangene und kommende Spiele der Liga aus der API
        /// </summary>
        private async Task LoadGamesAsync()
        {
            LoggerService.Logger.Information("Lade Spiele für Liga: {0} ({1})", _league.LeagueName, _league.LeagueId);

            try
            {
                /// <summary>
                /// Vergangene 5 Spiele anzeigen
                /// </summary>
                var past = await ApiFootballService.GetPastMatchesAsync(_league.LeagueId, _league.Season, last: 5);

                /// <summary>
                /// Kommende 10 Spiele anzeigen
                /// </summary>
                var upcoming = await ApiFootballService.GetUpcomingMatchesAsync(_league.LeagueId, _league.Season, next: 10);

                /// <summary>
                /// Fallback: Spiele innerhalb von 30 Tagen falls keine vorhanden
                /// </summary>
                if (upcoming == null || !upcoming.Any())
                {
                    upcoming = await ApiFootballService.GetMatchesByDateRangeAsync(_league.LeagueId, _league.Season, days: 30);
                }

                /// <summary>
                /// Liste anzeigen
                /// </summary>
                ListBoxPastSpiele.ItemsSource = past;
                ListBoxLiveSpiele.ItemsSource = upcoming;

                /// <summary>
                /// Hinweis anzeigen falls keine Spiele vorhanden sind
                /// </summary>
                if (past.Any() || upcoming.Any())
                {
                    KeineSpieleTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    KeineSpieleTextBlock.Visibility = Visibility.Visible;
                }

                LoggerService.Logger.Information("Spiele geladen für Liga: {0} - Vergangenheit: {1}, Zukunft: {2}",
                    _league.LeagueName, past?.Count ?? 0, upcoming?.Count ?? 0);
            }
            catch (Exception ex)
            {
                LoggerService.Logger.Error(ex, "Fehler beim Laden der Spiele für Liga: {0}", _league.LeagueName);
            }
        }

        /// <summary>
        /// Öffnet ein neues Fenster mit Spiel-Details
        /// </summary>
        private void ListBoxGame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox)?.SelectedItem is LiveMatchResponse match)
            {
                var detailFenster = new SpielDetailsWindow(match);
                detailFenster.Show();
                LoggerService.Logger.Information("SpielDetailsWindow geöffnet für FixtureId: {0}", match.FixtureId);
            }
        }
    }
}