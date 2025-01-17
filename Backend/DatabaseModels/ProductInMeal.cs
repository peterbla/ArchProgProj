using System.Runtime.CompilerServices;

using Backend.Services;

using Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.DatabaseModels
{
    [Table("ProductsInMeals")]
    public class ProductInMeal : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("meal_entry_id")]
        public int MealEntryId { get; set; }

        [Column("amount")]
        public int AmountG { get; set; }

        public async Task<ReturnedProductInMeal> ConwertToReturn(ProductService productService)
        {
            ReturnedProduct product = await productService.GetOneProduct(ProductId);

            ReturnedProductInMeal ret = new()
            {
                Id = Id,
                ProductId = ProductId,
                MealEntryId = MealEntryId,
                AmountG = AmountG,
                ProductName = product.Name,
                Energy = (int)(AmountG/100f * product.Energy),
                Fat = AmountG/100f * product.Fat,
                Saturates = AmountG/100f * product.Saturates,
                Carbohydrate = AmountG/100f * product.Carbohydrate,
                Sugars = AmountG/100f * product.Sugars,
                Fibre = AmountG/100f * product.Fibre,
                Protein = AmountG/100f * product.Protein,
                Salt = AmountG/100f * product.Salt

            };
            return ret;
        }

    }
}
