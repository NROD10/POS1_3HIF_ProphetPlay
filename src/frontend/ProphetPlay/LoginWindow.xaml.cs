using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProphetPlay
{
    /// <summary>
    /// Login-Fenster – Wird als erstes angezeigt beim Programmstart.
    /// </summary>
    public partial class LoginWindow : Window
    {
        // Konstruktor → Fenster initialisieren
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Klick auf "Registrieren"-Button
        private void Button_Registrieren_Click(object sender, RoutedEventArgs e)
        {
            // Neues Fenster zum Registrieren öffnen
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Close(); // aktuelles Fenster schließen
        }

        // Klick auf "Einloggen"-Button
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Anmeldedaten aus Eingabefeldern lesen
            var model = new LoginModel
            {
                benutzername = UsernameBox.Text,
                passwort = PasswortBox.Password
            };

            using (HttpClient client = new HttpClient())
            {
                // API-Adresse setzen (Backend läuft lokal)
                client.BaseAddress = new Uri("http://localhost:8080/");

                // Objekt in JSON umwandeln
                var json = System.Text.Json.JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    // Login-Anfrage an das Backend senden
                    var response = await client.PostAsync("api/benutzer/login", content);

                    // Wenn erfolgreich → Benutzer begrüßen
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Antwort auslesen (JSON → Objekt)
                        var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                        MessageBox.Show($"Willkommen {loginResponse.benutzername} ({loginResponse.rolle})!");

                        // Hauptfenster öffnen mit Benutzer- und Rollendaten
                        new MainWindow(loginResponse.benutzername, loginResponse.rolle).Show();
                        this.Close(); // Loginfenster schließen
                    }
                    else
                    {
                        MessageBox.Show("Login fehlgeschlagen."); // falsche Daten
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message); // z.B. kein Server läuft
                }
            }
        }
    }
}
