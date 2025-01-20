using Models;
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
using System.Text.Json;


namespace Frontend
{

    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private async void Login_Click(object sender, RoutedEventArgs e)
        {

            string username = Username.Text;
            string password = Password.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            try
            {
                var loginResult = await LoginAsync(username, password);

                if (loginResult != null)
                {
                    AuthToken.CurrentUser = loginResult;
                    AuthToken.CurrentUser.Token = loginResult.Token;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}");
            }
        }


        private async Task<UserWithToken?> LoginAsync(string username, string password)
          {
              using HttpClient client = new();
              client.BaseAddress = new Uri("http://localhost:5019");
              client.DefaultRequestHeaders.Add("Accept", "application/json");

              var credentials = new { Name = username, Password = password };
              var json = JsonSerializer.Serialize(credentials);
              var content = new StringContent(json, Encoding.UTF8, "application/json");

              HttpResponseMessage response = await client.PostAsync("/User/login", content);

              if (response.IsSuccessStatusCode)
              {
                  string responseBody = await response.Content.ReadAsStringAsync();
                  return JsonSerializer.Deserialize<UserWithToken>(responseBody);
              }
              return null;
          }
          


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}

