using Backend.DatabaseModels;
using Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Frontend
{

    public partial class AddProductWindow : Window
    {

        public string Token = AuthToken.CurrentUser.Token.Substring(7);
        public ReturnedProduct SelectedProductId { get; private set; }
        public int SelectedAmount { get; private set; }
        private readonly MealType _mealType;


        public AddProductWindow(MealType mealType)
        {
            InitializeComponent();
            LoadProducts();
            _mealType = mealType;

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadProducts()
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

                HttpResponseMessage response = await client.GetAsync("/Products");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var products = JsonSerializer.Deserialize<List<ReturnedProduct>>(json, options);

                    ProductsListBox.ItemsSource = products;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to load products. Status: {response.StatusCode}. Message: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



        private async void AddProductToMeal(object sender, RoutedEventArgs e)
        {

            try
            {
                var newMeal = new NewMealEntry
                {
                    MealType = _mealType,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                };

                var createdMeal = await AddNewMeal(newMeal);

                if (createdMeal == null)
                {
                    return;
                }

                var selectedProduct = ProductsListBox.SelectedItem as ReturnedProduct;

                if (selectedProduct == null)
                {
                    return;
                }

                var productInMeal = new NewProductInMeal
                {
                    ProductId = selectedProduct.Id,
                    AmountG = Int32.Parse(AmountG.Text)
                };

                await AddProductToExistingMeal(createdMeal.Id, productInMeal);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var selectedProduct = radioButton?.DataContext as ReturnedProduct;

            if (selectedProduct != null)
            {
                ProductsListBox.SelectedItem = selectedProduct;
            }
        }
        private async Task<ReturnedMealEntry> AddNewMeal(NewMealEntry newMealEntry)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5019");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var jsonContent = JsonSerializer.Serialize(newMealEntry);

                var response = await client.PostAsJsonAsync("/Meals", newMealEntry);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var meal = await response.Content.ReadFromJsonAsync<ReturnedMealEntry>();
                    return meal;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    //MessageBox.Show($"Failed to create new meal. Status: {response.StatusCode}. Message: {errorMessage}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error creating meal: {ex.Message}");
                return null;
            }
        }


        private async Task AddProductToExistingMeal(int mealId, NewProductInMeal productInMeal)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5019");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = await client.PostAsJsonAsync($"/Meals/{mealId}/Products", productInMeal);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add product to meal. Status: {response.StatusCode}. Message: {errorMessage}");
            }
        }

        private void AddNewProduct(object sender, RoutedEventArgs e)
        {
            AddNewProduct addNew = new AddNewProduct();
            addNew.Show();
        }
    }

}
