using System.Runtime.CompilerServices;

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

        public ReturnedProductInMeal ConwertToReturn()
        {
            ReturnedProductInMeal ret = new()
            {
                Id = Id,
                ProductId = ProductId,
                MealEntryId = MealEntryId,
                AmountG = AmountG
            };
            return ret;
        }

    }
}
