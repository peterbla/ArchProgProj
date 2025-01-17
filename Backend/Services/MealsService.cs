using Backend.DatabaseModels;

using Models;

using Supabase;

using Supabase.Interfaces;
using Supabase.Postgrest.Responses;

namespace Backend.Services
{
    public class MealsService(Client supabaseClient, ProductService productService)
        
    {
        private readonly Client _supabaseClient = supabaseClient;
        private readonly ProductService _productService = productService;

        public async Task<List<ReturnedMealEntry>> GetAllUserMeals(User user)
        {
            ModeledResponse<MealEntry> mealEntries = await _supabaseClient.From<MealEntry>()
                .Where(x => x.UserId == user.Id)
                .Order(
                    "meal_type",
                    Supabase.Postgrest.Constants.Ordering.Descending,
                    Supabase.Postgrest.Constants.NullPosition.Last
                )
                .Order(
                    "date",
                    Supabase.Postgrest.Constants.Ordering.Descending,
                    Supabase.Postgrest.Constants.NullPosition.Last
                )
                .Get();
            return [.. (await Task.WhenAll(mealEntries.Models.Select(async x => await x.ConwertToReturn(this))))];
        }

        public async Task<ReturnedMealEntry> GetOneUserMeal(User user, int id)
        {
            MealEntry? mealEntry = await _supabaseClient.From<MealEntry>()
                .Where(x => x.UserId == user.Id && x.Id == id).Single();
            if (mealEntry == null)
            {
                throw new Exception("Meal doesn't exist.");
            }

            return await mealEntry.ConwertToReturn(this);
        }

        public async Task<ReturnedMealEntry> AddNewMeal(User user, NewMealEntry newMealEntry)
        {
            MealEntry mealEntry = new()
            {
                UserId = user.Id,
                MealType = newMealEntry.MealType,
                Date = newMealEntry.Date,
            };
            ModeledResponse<MealEntry> databaseMealEntry = await _supabaseClient.From<MealEntry>().Insert(mealEntry);

            return await databaseMealEntry.Model.ConwertToReturn(this);
        }

        public async Task<List<ReturnedProductInMeal>> GetAllProductsFromMeal(int mealId)
        {
            ModeledResponse<ProductInMeal> productsInMeal = await _supabaseClient.From<ProductInMeal>()
                .Where(x => x.MealEntryId == mealId)
                .Get();
            return [.. (await Task.WhenAll(
                productsInMeal.Models.Select(async x => await x.ConwertToReturn(_productService))
                ))];
        }

        public async Task<ReturnedProductInMeal> AddProductToMeal(User user, int mealId, NewProductInMeal newProductInMeal)
        {
            // Jeżeli nie wywali tutaj błędu, to oznacza że posiłek istnieje i należy do użytkownika, więc można dalej na nim pracować
            // Potencjał do zoptymalizowania
            await GetOneUserMeal(user, mealId);
            ProductInMeal productInMeal = new()
            {
                MealEntryId = mealId,
                ProductId = newProductInMeal.ProductId,
                AmountG = newProductInMeal.AmountG,
            };

            ModeledResponse<ProductInMeal> databaseProductInMeal = await _supabaseClient.From<ProductInMeal>().Insert(productInMeal);

            return await databaseProductInMeal.Model.ConwertToReturn(_productService);

        }
    }
}
