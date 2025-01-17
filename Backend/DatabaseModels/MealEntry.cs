using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

using Models;
using Backend.Services;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace Backend.DatabaseModels
{
    [Table("MealsEntries")]
    public class MealEntry : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("date")]
        public DateOnly Date { get; set; }
        [Column("meal_type")]
        public MealType MealType { get; set; }

        public async Task<ReturnedMealEntry> ConwertToReturn(MealsService mealsService)
        {
            List<ReturnedProductInMeal> productsInMeal = await mealsService.GetAllProductsFromMeal(Id);
            ProductsSummary productsSummary = productsInMeal.Aggregate(new ProductsSummary(), (ProductsSummary sum, ReturnedProductInMeal productInMeal) => sum.Add(productInMeal));
            ReturnedMealEntry ret = new()
            {
                Id = Id,
                UserId = UserId,
                Date = Date,
                MealType = MealType,
                OverallEnergy = productsSummary.Energy,
                OverallFat = productsSummary.Fat,
                OverallSaturates = productsSummary.Saturates,
                OverallCarbohydrate = productsSummary.Carbohydrate,
                OverallFibre = productsSummary.Fibre,
                OverallProtein = productsSummary.Protein,
                OverallSalt = productsSummary.Salt,
                OverallSugars = productsSummary.Sugars,
                ProductsInMeal = productsInMeal,
            };

            return ret;
        }
        private class ProductsSummary
        {
            public int Energy { get; set; } = 0;
            public float Fat { get; set; } = 0;
            public float Saturates { get; set; } = 0;
            public float Carbohydrate { get; set; } = 0;
            public float Sugars { get; set; } = 0;
            public float Fibre { get; set; } = 0;
            public float Protein { get; set; } = 0;
            public float Salt { get; set; } = 0;

            public ProductsSummary Add(ReturnedProductInMeal productInMeal)
            {
                Energy += productInMeal.Energy;
                Fat += productInMeal.Fat ?? 0;
                Saturates += productInMeal.Saturates ?? 0;
                Carbohydrate += productInMeal.Carbohydrate ?? 0;
                Sugars += productInMeal.Sugars ?? 0;
                Fibre += productInMeal.Fibre ?? 0;
                Protein += productInMeal.Protein ?? 0;
                Salt += productInMeal.Salt ?? 0;

                return this;
            }
        } 

    }
}
