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
            // 1) versuche next-Abfrage
            var games = await ApiFootballService.GetUpcomingMatchesAsync(_league.LeagueId, _league.Season, next: 10);

            // 2) Fallback auf Datum-Range, wenn leer
            if (games == null || !games.Any())
            {
                games = await ApiFootballService.GetMatchesByDateRangeAsync(_league.LeagueId, _league.Season, days: 30);
            }

            ListBoxLiveSpiele.ItemsSource = games;
            KeineSpieleTextBlock.Visibility = games.Any()
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ListBoxLiveSpiele_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListBoxLiveSpiele.SelectedItem is LiveMatchResponse match)
            {
                MessageBox.Show(
                    $"{match.TeamsString}\nZeit: {match.StartTime}\nStatus: {match.Status}",
                    "Spieldetails",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
