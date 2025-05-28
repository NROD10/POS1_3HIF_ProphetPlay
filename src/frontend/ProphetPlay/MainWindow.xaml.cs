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
            LoadNewsAsync(); // Nachrichten laden, wenn das Fenster geöffnet wird
        }

        private async Task LoadNewsAsync()
        {
            try
            {
                // API-Endpunkt (ersetze dies mit deiner API-URL)
                string apiUrl = "https://example.com/api/news";

                using HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(apiUrl);

                // JSON-Daten deserialisieren
                var newsItems = JsonSerializer.Deserialize<List<NewsItem>>(response);

                // ListBox mit den Nachrichten füllen
                NewsListBox.ItemsSource = newsItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Nachrichten: {ex.Message}");
            }
        }
    }

    
}