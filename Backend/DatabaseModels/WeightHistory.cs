using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.DatabaseModels
{
    [Table("WeightsHistory")]
    public class WeightHistory : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("datetime")]
        public DateTime Datetime { get; set; }
        [Column("weight")]
        public float WeightKg { get; set; }
    }
}
