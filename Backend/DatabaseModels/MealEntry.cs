using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

using Models;

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
        public DateTime Date { get; set; }
        [Column("meal_type")]
        public MealType MealType { get; set; }

    }
}
