using System.Windows;
using System.Windows.Media.Imaging;


namespace ProphetPlay
{
    /// <summary>
    /// Fenster zur Anzeige von Details des Fußballspiels
    /// Zeigt Teams, Spielzeit und Spielereignisse wie Tore, Karten, Auswechslung, usw.
    /// </summary>
    public partial class SpielDetailsWindow : Window
    {

        /// <summary>
        /// Das Spiel, dessen Details angezeigt werden
        /// </summary>
        private readonly LiveMatchResponse _match;

        /// <param name="match">Das ausgewählte Live-Spiel dessen Details angezeigt werden sollen</param>
        public SpielDetailsWindow(LiveMatchResponse match)
        {
            InitializeComponent();
            _match = match;
            LoggerService.Logger.Information("SpielDetailsWindow geöffnet für FixtureId: {0}", _match.FixtureId);
            LoadDetails();
        }

        /// <summary>
        /// Lädt die Spieldetails wie Teamnamen, Spielzeit, Ereignisse und zeigt sie im Fenster an
        /// </summary>
        private async void LoadDetails()
        {
            LoggerService.Logger.Information("Lade Details für FixtureId: {0}", _match.FixtureId);

            TeamsHeader.Text = _match.TeamsString;
            TimeHeader.Text = _match.MatchDateTime;

            try
            {
                var events = await ApiFootballService.GetFixtureEventsAsync(_match.FixtureId);
                EventsList.ItemsSource = events;
                LoggerService.Logger.Information("Ereignisse geladen für FixtureId: {0}, Anzahl Ereignisse: {1}", _match.FixtureId, events?.Count ?? 0);
            }
            catch (Exception ex)
            {
                LoggerService.Logger.Error(ex, "Fehler beim Laden der Ereignisse für FixtureId: {0}", _match.FixtureId);
            }
        }
    }
}