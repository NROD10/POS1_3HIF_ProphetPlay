using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
            // Falls du später dynamisch befüllen willst:
            // RollenBox.ItemsSource = new[] { "Admin", "User" };
            // RollenBox.SelectedIndex = 1; // Default "User"
        }

        private void ZurueckButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        // jaja

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Rolle aus der Combo in die korrekte ID mappen
            string roleName = ((ComboBoxItem)RollenBox.SelectedItem).Content.ToString();
            int roleId = roleName == "Admin" ? 1 : 2;

            var model = new RegisterModel
            {
                benutzername = UsernameBox.Text,
                passwort = PasswortBox.Password,
                role_id = roleId
            };

            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
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
                else
                {
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
                MessageBox.Show($"Fehler beim Registrieren:\n{ex.Message}",
                                "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
