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

namespace Frontend
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public async void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Password;
            int Height = int.TryParse(Height_cm.Text, out var height) ? height : 0;
            float Weight = float.TryParse(Weight_kg.Text, out var weight) ? weight : 0;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                var isRegistered = await RegisterAsync(username, password, height, weight);


                if (isRegistered)
                {

                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Registration failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}");
            }
        }

        private async Task<bool> RegisterAsync(string username, string password, int height, float weight)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5019"); 
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var newUser = new
                {
                    Name = username,
                    Password = password,
                    HeightCm = height,
                    WeightKg = weight
                };

                var json = System.Text.Json.JsonSerializer.Serialize(newUser);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/User/register", content);

            
                return response.IsSuccessStatusCode;
            }
        }
        
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
