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
    /// Stellt das Login-Fenster des Programms dar
    /// Wird beim Start des Programms angezeigt und ermöglicht Benutzern die Anmeldung
    /// </summary>
    /// 
    // Prompt: "Erstell mir ein WPF-Login-Fenster mit Username- und Passwort-Feld und nem Button, der die Daten per HTTP-Post zum Backend schickt und bei erfolgreichem Login das MainWindow aufmacht"

    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen wenn der Benutzer auf den Registrieren-Button klickt
        /// Öffnet das Registrierungsfenster und schließt das Loginfenster
        /// </summary>        
        private void Button_Registrieren_Click(object sender, RoutedEventArgs e)
        {
            // Neues Fenster zum Registrieren öffnen
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Close(); // aktuelles Fenster schließen
        }

        /// <summary>
        /// Wird aufgerufen wenn der Benutzer auf den Einloggen-Button klickt
        /// Sendet die Anmeldedaten an das Backend, prüft die Antwort und öffnet das Hauptfenster bei Erfolg
        /// </summary>

        // Prompt: schreib jetzt die LoginButton_Click-Methode, die user und passwort, ans API schickt und Fehlermeldung anzeigt"
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
                    LoggerService.Logger.Information("Login-Versuch von Benutzer: {0}", model.benutzername);
                    var response = await client.PostAsync("api/benutzer/login", content);


                    // Wenn erfolgreich dann Benutzer begrüßen
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Antwort auslesen (JSON → Objekt)
                        var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                        // Hauptfenster öffnen mit Benutzer- und Rollendaten
                        new MainWindow(loginResponse.benutzername, loginResponse.rolle).Show();
                        this.Close(); // Loginfenster schließen
                    }
                    else
                    {
                        LoggerService.Logger.Warning("Login fehlgeschlagen für Benutzer: {0}", model.benutzername);
                        MessageBox.Show("Login fehlgeschlagen."); // falsche Daten
                    }
                }
                catch (Exception ex)
                {
                    LoggerService.Logger.Error(ex, "Fehler beim Login");
                    MessageBox.Show("Fehler: " + ex.Message); // z.B. kein Server läuft
                }
            }
        }
    }
}