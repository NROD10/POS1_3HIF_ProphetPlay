// CreateNewsWindow.xaml.cs
using System.Windows;

namespace ProphetPlay
{
    public partial class CreateNewsWindow : Window
    {
        public string CurrentUser { get; set; }

        public CreateNewsWindow(string aktuellerBenutzername)
        {
            InitializeComponent();
            CurrentUser = aktuellerBenutzername;
        }

        // Abbrechen → DialogResult = false
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Erstellen → API-Call, bei Erfolg DialogResult = true
        private async void OnCreate(object sender, RoutedEventArgs e)
        {
            var dto = new NewsArticleCreate
            {
                Title = TxtTitle.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Url = TxtUrl.Text.Trim()
            };

            bool ok = await NewsService.CreateNewsAsync(dto, CurrentUser);
            if (ok)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Fehler beim Anlegen der News.",
                                "Fehler",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
