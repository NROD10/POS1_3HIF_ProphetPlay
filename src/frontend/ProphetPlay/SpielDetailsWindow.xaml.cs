using System.Windows;
using System.Windows.Media.Imaging;

namespace ProphetPlay
{
    public partial class SpielDetailsWindow : Window
    {
        private readonly LiveMatchResponse _match;

        public SpielDetailsWindow(LiveMatchResponse match)
        {
            InitializeComponent();
            _match = match;
            LoadDetails();
        }

        private async void LoadDetails()
        {
            // Header befüllen
            TeamsHeader.Text = _match.TeamsString;
            TimeHeader.Text = _match.MatchDateTime;

            // Events laden
            var events = await ApiFootballService.GetFixtureEventsAsync(_match.FixtureId);
            EventsList.ItemsSource = events;
        }
    }
}
