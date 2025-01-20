using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace Frontend
{
    public partial class MealHistory : Page
    {
        public MealHistory()
        {
            InitializeComponent();
            LoadMealHistory();
        }

        public string Token = AuthToken.CurrentUser.Token.Substring(7);

        private async void LoadMealHistory()
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

                HttpResponseMessage response = await client.GetAsync("/Meals");
                ;

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var mealHisotry = JsonSerializer.Deserialize<List<ReturnedMealEntry>>(json, options);

                    MealHistoryDataGrid.ItemsSource = mealHisotry;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to load meal history. Status: {response.StatusCode}. Message: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
 
    }
}
