using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void ZurueckButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new RegisterModel
            {
                benutzername = UsernameBox.Text,
                passwort = PasswortBox.Password,
                rolle = RollenBox.Text
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                var json = System.Text.Json.JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("api/benutzer/register", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Registrierung erfolgreich!");
                        new LoginWindow().Show();
                        this.Close();
                    }
                    else if ((int)response.StatusCode == 409)
                    {
                        MessageBox.Show("Benutzername existiert bereits.");
                    }
                    else
                    {
                        MessageBox.Show("Registrierung fehlgeschlagen.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

    }
}