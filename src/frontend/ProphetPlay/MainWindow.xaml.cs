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
        public MainWindow()
        {
            InitializeComponent();
            LoadNews(); // Nachrichten laden, wenn das Fenster geöffnet wird
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

    }

    
}