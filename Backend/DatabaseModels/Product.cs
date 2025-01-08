using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.DatabaseModels
{
    [Table("Products")]
    public class Product : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("energy")]
        public float Energy { get; set; }
        [Column("fat")]
        public float? Fat { get; set; }
        [Column("fat_saturates")]
        public float? Saturates { get; set; }
        [Column("carbohydrate")]
        public float? Carbohydrate { get; set; }
        [Column("sugars")]
        public float? Sugars { get; set; }
        [Column("fibre")]
        public float? Fibre { get; set; }
        [Column("protein")]
        public float? Protein { get; set; }
        [Column("salt")]
        public float? Salt { get; set; }
    }
}
