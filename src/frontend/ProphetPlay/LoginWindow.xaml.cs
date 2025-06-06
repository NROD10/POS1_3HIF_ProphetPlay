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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Registrieren_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Close();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new LoginModel
            {
                benutzername = UsernameBox.Text,
                passwort = PasswortBox.Password
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                var json = System.Text.Json.JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("api/benutzer/login", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                        MessageBox.Show($"Willkommen {loginResponse.benutzername} ({loginResponse.rolle})!");
                        new MainWindow().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login fehlgeschlagen.");
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
