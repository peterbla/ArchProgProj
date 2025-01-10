namespace Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("MealsHistory")]
class MealsHistory : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public string UserId { get; set; }

    [Column("date")]
    public DateTime? Date { get; set; }

    [Column("meal")]
    public MealType? Meal { get; set; }
    
    [Column("product_string")]
    public string? ProductString { get; set; }
    
    [Column("product_list")]
    public int[]? ProductList { get; set; }
}