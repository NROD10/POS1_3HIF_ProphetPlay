using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ProphetPlay
{
    /// <summary>
    /// Code-Behind für das Registrierungsfenster (GUI)
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        // Klick auf "Zurück" → zurück zum Login-Fenster
        private void ZurueckButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();  // Neues Login-Fenster öffnen
            this.Close();              // Registrierungsfenster schließen
        }

        // Klick auf "Registrieren"-Button
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Rolle aus ComboBox ("Admin" oder "User") in ID umwandeln
            string roleName = ((ComboBoxItem)RollenBox.SelectedItem).Content.ToString();
            int roleId = roleName == "Admin" ? 1 : 2;

            // Benutzer-Objekt vorbereiten
            var model = new RegisterModel
            {
                benutzername = UsernameBox.Text,      // Eingabe Benutzername
                passwort = PasswortBox.Password,      // Eingabe Passwort
                role_id = roleId                      // Rolle (1 = Admin, 2 = User)
            };

            // HTTP-Client vorbereiten für Anfrage an Backend
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };

            // Objekt in JSON umwandeln
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Anfrage an API senden → POST an /api/benutzer/register
                var response = await client.PostAsync("api/benutzer/register", content);

                if (response.IsSuccessStatusCode)
                {
                    // Erfolg → Meldung + Weiterleitung zum Login
                    MessageBox.Show("Registrierung erfolgreich!");
                    new LoginWindow().Show();
                    this.Close();
                }
                else
                {
                    // Fehlertext vom Server lesen
                    var err = await response.Content.ReadAsStringAsync();
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
                // z.B. keine Verbindung zum Server
                MessageBox.Show($"Fehler beim Registrieren:\n{ex.Message}",
                                "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
