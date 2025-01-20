using Backend.DatabaseModels;
using Models;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Frontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            string dateStr = date.ToString("yyyy-MM-dd");
            DateButton.Content = date.ToString("dd/MM/yyyy");
            //LoadMealsForDate(date);
            UpdateDateAndLoadMeals();

        }
        public string Token = AuthToken.CurrentUser.Token.Substring(7);
        private DateTime date = DateTime.Today;
        private async void LoadMealsForDate(DateTime date)
        {
            try
            {
                var meals = await GetMealsForDate(date);

                if (meals != null && meals.Any())
                {
                    BindMeals(meals);
                }
                else
                {
                    ClearMealBindings();
                    //MessageBox.Show("No meals found for the selected date.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading meals: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<ReturnedMealEntry>> GetMealsForDate(DateTime date)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5019");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                    string dateStr = date.ToString("yyyy-MM-dd");
                    HttpResponseMessage response = await client.GetAsync($"/Meals?date={dateStr}");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        return JsonSerializer.Deserialize<List<ReturnedMealEntry>>(json, options);
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Status: {response.StatusCode}, Message: {errorMessage}");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("Failed to connect to the server. Please check your connection.", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching meals.", ex);
            }
        }

        private void BindMeals(List<ReturnedMealEntry> meals)
        {
            var breakfastProducts = GetProductsForMealType(meals, MealType.Breakfast);
            var lunchProducts = GetProductsForMealType(meals, MealType.Lunch);
            var dinnerProducts = GetProductsForMealType(meals, MealType.Dinner);
            var supperProducts = GetProductsForMealType(meals, MealType.Supper);

            UpdateProductList(ProductsList0, breakfastProducts, summaryBlockBreakfast);
            UpdateProductList(ProductsList1, lunchProducts, summaryBlockLunch);
            UpdateProductList(ProductsList2, dinnerProducts, summaryBlockDinner);
            UpdateProductList(ProductsList3, supperProducts, summaryBlockSupper);

            UpdateDailySummary(
            new ObservableCollection<ReturnedProduct>(breakfastProducts),
            new ObservableCollection<ReturnedProduct>(lunchProducts),
            new ObservableCollection<ReturnedProduct>(dinnerProducts),
            new ObservableCollection<ReturnedProduct>(supperProducts),
            summaryBlockDaily
);
        }

        private List<ReturnedProduct> GetProductsForMealType(List<ReturnedMealEntry> meals, MealType mealType)
        {
            return meals.Where(m => m.MealType == mealType)
                        .SelectMany(m => m.ProductsInMeal)
                        .Select(MapToReturnedProduct)
                        .ToList();
        }

        private void UpdateProductList(ListBox listBox, List<ReturnedProduct> products, TextBlock summaryBlock)
        {
            listBox.ItemsSource = products;
            UpdateMealSummary(new ObservableCollection<ReturnedProduct>(products), summaryBlock);
        }

        private void ClearMealBindings()
        {
            ProductsList0.ItemsSource = null;
            ProductsList1.ItemsSource = null;
            ProductsList2.ItemsSource = null;
            ProductsList3.ItemsSource = null;

            summaryBlockBreakfast.Text = "0 kcal |  0 protein | 0 fat | 0 carbs ";
            summaryBlockLunch.Text = "0 kcal |  0 protein | 0 fat | 0 carbs ";
            summaryBlockDinner.Text = "0 kcal |  0 protein | 0 fat | 0 carbs ";
            summaryBlockSupper.Text = "0 kcal |  0 protein | 0 fat | 0 carbs ";
        }
        private void UpdateMealSummary(ObservableCollection<ReturnedProduct> mealProducts, TextBlock summaryBlock)
        {
            if (mealProducts == null || !mealProducts.Any())
            {
                summaryBlock.Text = "0 kcal  |   0g protein  |  0g fat  |  0g carbs  ";
                return;
            }

            var totalEnergy = mealProducts.Sum(p => p.Energy);
            var totalProtein = mealProducts.Sum(p => p.Protein ?? 0);
            var totalFat = mealProducts.Sum(p => p.Fat ?? 0);
            var totalCarbs = mealProducts.Sum(p => p.Carbohydrate ?? 0);

            summaryBlock.InvalidateVisual();

            summaryBlock.Text = $"{totalEnergy}g kcal   |   {totalProtein}g protein   |   {totalFat}g fat   |   {totalCarbs}g carbs";
        }

        private void UpdateDailySummary(
                    ObservableCollection<ReturnedProduct> breakfastProducts,
                    ObservableCollection<ReturnedProduct> lunchProducts,
                    ObservableCollection<ReturnedProduct> dinnerProducts,
                    ObservableCollection<ReturnedProduct> supperProducts,
                    TextBlock summaryBlock)

        {
            var allProducts = breakfastProducts.Concat(lunchProducts)
                                               .Concat(dinnerProducts)
                                               .Concat(supperProducts);

            var totalEnergy = allProducts.Sum(p =>p.Energy);
            var totalProtein = allProducts.Sum(p => p.Protein ?? 0);
            var totalFat = allProducts.Sum(p => p.Fat ?? 0);
            var totalCarbs = allProducts.Sum(p => p.Carbohydrate ?? 0);


            summaryBlock.Text = $"{totalEnergy} kcal | {totalProtein}g protein | {totalFat}g fat | {totalCarbs}g carbs";
        }


        private ReturnedProduct MapToReturnedProduct(ReturnedProductInMeal productInMeal)
        {
            return new ReturnedProduct
            {
                Name = productInMeal.ProductName,
                Energy = productInMeal.Energy,
                Protein = productInMeal.Protein,
                Fat = productInMeal.Fat,
                Carbohydrate = productInMeal.Carbohydrate
            };
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Page_WeightHistory(object sender, RoutedEventArgs e)
        {
            Main.Content = new WeightHistory();
        }

        private void Page_MealsHistory(object sender, RoutedEventArgs e)
        {
            Main.Content = new MealHistory();
        }

        private void Page_Main(object sender, RoutedEventArgs e)
        {
            Main.Content = null;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AuthToken.CurrentUser = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }


        private void DateLeft_Click(object sender, RoutedEventArgs e)
        {
            date = date.AddDays(-1);
            DateButton.Content = date.ToString("dd/MM/yyyy");
            UpdateDateAndLoadMeals();
        }

        /*private void DateButton_Click(object sender, RoutedEventArgs e)
        {
            
            UpdateDateAndLoadMeals();
        }*/

        private void DateRight_Click(object sender, RoutedEventArgs e)
        {
            date = date.AddDays(1);
            DateButton.Content = date.ToString("dd/MM/yyyy");
            UpdateDateAndLoadMeals(); ;
        }
        private void UpdateDateAndLoadMeals()
        {
            DateButton.Content = date.ToString("dd/MM/yyyy");
            LoadMealsForDate(date);
        }

        private void AddBreakfast_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(MealType.Breakfast);
            addProductWindow.ShowDialog();
        }
        
        private void AddLunch_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(MealType.Lunch);
            addProductWindow.ShowDialog();
        }

        private void AddDinner_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(MealType.Dinner);
            addProductWindow.ShowDialog();
        }

        private void AddSupper_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(MealType.Supper);
            addProductWindow.ShowDialog();
        }

        private void DateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
