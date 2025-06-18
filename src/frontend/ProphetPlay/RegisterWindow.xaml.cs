using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace ProphetPlay
{
    /// <summary>
    /// Das Registrierungsfenster für neue Benutzer
    /// </summary>
    public partial class RegisterWindow : Window
    {

        public RegisterWindow()
        {
            InitializeComponent();
            LoggerService.Logger.Information("RegisterWindow geöffnet");
        }

        /// <summary>
        /// Wenn der Zurückbutton geklickt wird öffnet sich das Login-Fenster und schließt das aktuelle Fenster
        /// </summary>
        private void ZurueckButton_Click(object sender, RoutedEventArgs e)
        {
            LoggerService.Logger.Information("Zurück-Button geklickt, RegisterWindow wird geschlossen, LoginWindow geöffnet");
            new LoginWindow().Show();
            this.Close();
        }

        /// <summary>
        /// Wenn der Registrierbutton geklickt wird bereitet das Benutzerobjekt vor und sendet es per POST an die API
        /// Erfolgreiche Registrierung führt zurück zum Login-Fenster.
        /// </summary>
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Rolle aus ComboBox ("Admin" oder "User") in ID umwandeln
            string rollenName = ((ComboBoxItem)RollenBox.SelectedItem).Content.ToString();
            int rollenId = 0;

            if (rollenName == "Admin")
            {
                rollenId = 1;
            }
            else
            {
                rollenId = 2;
            }

            /// <summary>
            /// Modell für die Registrierung mit Benutzername, Passwort, Rolle
            /// </summary>
            var model = new RegisterModel
            {
                benutzername = UsernameBox.Text,
                passwort = PasswortBox.Password,
                role_id = rollenId
            };

            LoggerService.Logger.Information("Registrierungsversuch für Benutzer: {0} mit Rolle: {1}", model.benutzername, rollenName);

            /// <summary>
            /// HTTP-Client für POST-Anfrage vorbereiten
            /// </summary>
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };

            /// <summary>
            /// Objekt in JSON umwandeln
            /// </summary>
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                /// <summary>
                /// POST-Anfrage an /api/benutzer/register senden und bei Erfolg wird der Benutzer registriert
                /// </summary>
                var response = await client.PostAsync("api/benutzer/register", content);

                if (response.IsSuccessStatusCode)
                {
                    LoggerService.Logger.Information("Registrierung erfolgreich für Benutzer: {0}", model.benutzername);
                    MessageBox.Show("Registrierung erfolgreich!");
                    new LoginWindow().Show();
                    this.Close();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    LoggerService.Logger.Warning("Registrierung fehlgeschlagen für Benutzer: {0}. Status: {1}, Fehler: {2}",
                        model.benutzername, (int)response.StatusCode, err);

                    MessageBox.Show(
                        $"Registrierung fehlgeschlagen ({(int)response.StatusCode}):\n{err}",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger.Error(ex, "Exception beim Registrieren für Benutzer: {0}", model.benutzername);
                MessageBox.Show($"Fehler beim Registrieren:\n{ex.Message}",
                                "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}