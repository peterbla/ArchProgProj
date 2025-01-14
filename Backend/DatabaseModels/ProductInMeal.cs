using System.Runtime.CompilerServices;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.DatabaseModels
{
    [Table("ProductsInMeals")]
    public class ProductInMeal
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("meal_entry_id")]
        public int MealEntryId { get; set; }
        [Column("amount")]
        public int AmountG { get; set; }

    }
}
