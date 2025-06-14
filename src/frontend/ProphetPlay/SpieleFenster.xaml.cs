using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProphetPlay
{
    public partial class SpieleFenster : Window
    {
        private readonly LeaguesArticle _league;

        public SpieleFenster(LeaguesArticle league)
        {
            InitializeComponent();
            _league = league;

            ligen_ueberschrift_textblock.Text = league.LeagueName;
            land_ueberschrift_textblock.Text = league.CountryName;
            logo_image.Source = new BitmapImage(new Uri(league.LogoUrl));

            Loaded += async (_, __) => await LoadGamesAsync();
        }

        private async Task LoadGamesAsync()
        {
            // 1) letzte Spiele
            var past = await ApiFootballService.GetPastMatchesAsync(_league.LeagueId, _league.Season, last: 5);

            // 2) aktuelle/Kommende Spiele
            var upcoming = await ApiFootballService.GetUpcomingMatchesAsync(_league.LeagueId, _league.Season, next: 10);
            if (upcoming == null || !upcoming.Any())
            {
                upcoming = await ApiFootballService.GetMatchesByDateRangeAsync(_league.LeagueId, _league.Season, days: 30);
            }

            // 3) anzeigen
            ListBoxPastSpiele.ItemsSource = past;
            ListBoxLiveSpiele.ItemsSource = upcoming;
            KeineSpieleTextBlock.Visibility = (past.Any() || upcoming.Any())
                                              ? Visibility.Collapsed
                                              : Visibility.Visible;
        }

        private void ListBoxLiveSpiele_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is LiveMatchResponse match)
            {
                MessageBox.Show(
                    $"{match.TeamsString}\n" +
                    $"Datum: {match.MatchDateTime}\n" +
                    $"Ergebnis: {match.DisplayScore}\n" +
                    $"Status: {match.Status}",
                    "Spieldetails",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
