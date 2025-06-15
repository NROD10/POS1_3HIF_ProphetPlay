using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        // Gemeinsamer Handler für Double-Click auf beide ListBoxes
        private void ListBoxGame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as System.Windows.Controls.ListBox)?.SelectedItem is LiveMatchResponse match)
            {
                var detailFenster = new SpielDetailsWindow(match);
                detailFenster.Show();
                // jaja
            }
        }
    }
}