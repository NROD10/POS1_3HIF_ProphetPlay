// CreateNewsWindow.xaml.cs
using System.Windows;

namespace ProphetPlay
{
    /// <summary>
    /// Fenster zum Erstellen eines neuen Newsartikels
    /// </summary>
    public partial class CreateNewsWindow : Window
    {
        /// <summary>
        /// Der aktuell eingeloggte Benutzername
        /// </summary>
        public string CurrentUser { get; set; }

        /// <summary>
        /// Initialisiert das Fenster mit dem aktuellen Benutzernamen
        /// </summary>
        /// <param name="aktuellerBenutzername">Der Benutzername des eingeloggten Nutzers</param>
        public CreateNewsWindow(string aktuellerBenutzername)
        {
            LoggerService.Logger.Information("CreateNewsWindow wird initialisiert für Benutzer: {Benutzer}", aktuellerBenutzername);
            InitializeComponent();
            CurrentUser = aktuellerBenutzername;
        }

        /// <summary>
        /// Schließt das Fenster ohne eine News zu erstellen
        /// </summary>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            LoggerService.Logger.Information("News-Erstellung abgebrochen von Benutzer: {Benutzer}", CurrentUser);
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Erstellt eine neue News über die API
        /// </summary>
        private async void OnCreate(object sender, RoutedEventArgs e)
        {
            var dto = new NewsArticleCreate
            {
                Title = TxtTitle.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Url = TxtUrl.Text.Trim()
            };

            LoggerService.Logger.Information("News-Erstellungsversuch durch {Benutzer}: {@NewsDTO}", CurrentUser, dto);

            bool ok = await NewsService.CreateNewsAsync(dto, CurrentUser);
            if (ok)
            {
                LoggerService.Logger.Information("News erfolgreich erstellt durch {Benutzer}", CurrentUser);
                DialogResult = true;
                Close();
            }
            else
            {
                LoggerService.Logger.Warning("Fehler bei der News-Erstellung durch {Benutzer}", CurrentUser);
                MessageBox.Show("Fehler beim Anlegen der News.",
                                "Fehler",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
