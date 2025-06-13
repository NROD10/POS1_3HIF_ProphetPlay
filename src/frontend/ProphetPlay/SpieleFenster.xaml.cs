using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProphetPlay
{
    /// <summary>
    /// Interaction logic for SpieleFenster.xaml
    /// </summary>
    public partial class SpieleFenster : Window
    {
        private LeaguesArticle _league;
        public SpieleFenster(LeaguesArticle league)
        {
            InitializeComponent();
            _league = league;
            // Nutze _league.LeagueName etc.
            LoadLeagues_ueberschrift();
            ShowLiveGames();
        }


        private async void LoadLeagues_ueberschrift()
        {
            try
            {
                ligen_ueberschrift_textblock.Text = _league.LeagueName;
                land_ueberschrift_textblock.Text = _league.CountryName;
                logo_image.Source = new BitmapImage(new Uri(_league.LogoUrl));

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Leaguen: {ex.Message}");
            }
        }
        
        private async void ShowLiveGames()
        {
            try
            {
                var leagueId = _league.LeagueId; 
                var games = await ApiFootballService.GetLiveMatchesAsync(leagueId);

                ListBoxLiveSpiele.ItemsSource = games;

                // Sichtbarkeit umschalten
                if (games == null || games.Count == 0)
                {

                    KeineSpieleTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    KeineSpieleTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Spiele: {ex.Message}");
                KeineSpieleTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void SpieleButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                LiveMatchesApiResponse livematch = btn.DataContext as LiveMatchesApiResponse;
                if (livematch != null)
                {
                    //DeteilSpielFenster fenster = new DeteilSpielFenster(livematch);
                    //fenster.Show();
                }
            }

        }


        private async Task ShowMatchesByDate(string date)
        {
            try
            {
                var allMatches = await ApiFootballService.GetMatchesByDateAsync(date);

                var liveMatches = allMatches
                    .Where(m => m.Fixture?.Status?.Short == "LIVE" ||
                                m.Fixture?.Status?.Short == "1H" ||
                                m.Fixture?.Status?.Short == "2H")
                    .ToList();

                var dateMatches = allMatches
                    .Where(m => m.Fixture?.Status?.Short != "LIVE" &&
                                m.Fixture?.Status?.Short != "1H" &&
                                m.Fixture?.Status?.Short != "2H")
                    .ToList();

                ListBoxLiveSpiele.ItemsSource = liveMatches;
                ListBoxDatumSpiele.ItemsSource = dateMatches;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Spiele: {ex.Message}");
            }
        }



        private async void Kalender_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kalender.SelectedDate.HasValue)
            {
                string selectedDate = Kalender.SelectedDate.Value.ToString("yyyy-MM-dd");
                await ShowMatchesByDate(selectedDate);
            }
        }




    }
}
