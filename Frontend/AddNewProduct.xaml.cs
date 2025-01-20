using Backend.DatabaseModels;
using Backend.Services;
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
using System.Windows.Shapes;
using System.Net.Http.Json;

namespace Frontend
{
    public partial class AddNewProduct : Window
    {
        public string Token = AuthToken.CurrentUser.Token.Substring(7);

        public AddNewProduct()
        {
            InitializeComponent();
        }

        private async void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newProduct = new NewProduct
                {
                    Name = NameTextBox.Text,
                    Energy = int.Parse(EnergyTextBox.Text),
                    Fat = ParseNullableFloat(FatTextBox.Text),
                    Carbohydrate = ParseNullableFloat(CarbohydrateTextBox.Text),
                    Saturates = ParseNullableFloat(SaturatesTextBox.Text),
                    Protein = ParseNullableFloat(ProteinTextBox.Text),
                    Sugars = ParseNullableFloat(SugarsTextBox.Text),
                    Fibre = ParseNullableFloat(FibreTextBox.Text),
                    Salt = ParseNullableFloat(SaltTextBox.Text),
                };


                await AddProduct(newProduct);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            Close();
        }

        public async Task<ReturnedProduct> AddProduct(NewProduct newProduct)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5019");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = await client.PostAsJsonAsync($"/Products", newProduct);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add product to meal. Status: {response.StatusCode}. Message: {errorMessage}");
            }
            else
            {
                return null;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private float? ParseNullableFloat(string value)
        {
            return float.TryParse(value, out var result) ? result : null;
        }


    }

}
