using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend
{

    public partial class WeightHistory : Page
    {
        public string Token = AuthToken.CurrentUser.Token.Substring(7);


        public WeightHistory()
        {
            InitializeComponent();
            LoadWeightHistory();
        }


        private async void LoadWeightHistory()
        {
            if (string.IsNullOrEmpty(Token))
            {
                MessageBox.Show("Unauthorized. Please log in.");
                return;
            }

            try
            {
                using HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5019");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                HttpResponseMessage response = await client.GetAsync("/WeightHistory");
                ;

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                

                    var weightHistory = JsonSerializer.Deserialize<List<ReturnedWeightHistory>>(json, options);

                    WeightHistoryDataGrid.ItemsSource = weightHistory;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to load weight history. Status: {response.StatusCode}. Message: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private async void AddWeight_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Token))
            {
                MessageBox.Show("Unauthorized. Please log in.");
                return;
            }

            if (!float.TryParse(WeightInput.Text, out var weight) || weight <= 0)
            {
                MessageBox.Show("Please enter a valid weight.");
                return;
            }

            try
            {
                using HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5019");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var newWeightEntry = new NewWeightHistory
                {
                    WeightKg = weight,
                    Datetime = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(newWeightEntry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/WeightHistory", content);

                if (response.IsSuccessStatusCode)
                {
                    LoadWeightHistory();
                }
                else
                {
                    MessageBox.Show($"Failed to add weight entry. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding weight: {ex.Message}");
            }
        }



    }
}
