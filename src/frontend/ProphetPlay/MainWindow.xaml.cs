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


namespace ProphetPlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> NewsItems { get; set; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadNewsAsync();
        }

        private async void LoadNewsAsync()
        {
            var api = new ApiFootballService();
            var newsData = await api.GetNewsAsync();

            if (newsData?["response"] != null)
            {
                NewsItems.Clear();
                foreach (var article in newsData["response"])
                {
                    string title = article["title"]?.ToString();
                    NewsItems.Add(title);
                }
            }
            else
            {
                NewsItems.Add("⚠️ Keine Nachrichten verfügbar.");
            }
        }
    }
}