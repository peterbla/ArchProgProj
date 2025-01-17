using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.DatabaseModels
{
    [Table("Users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Name { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("height")]
        public int? HeightCm { get; set; }
        [Column("weight")]
        public float? WeightKg { get; set; }
    }
}
