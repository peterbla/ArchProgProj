using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;

namespace Models
{
    [Table("WeightsHistory")]
    public class NewWeightHistory
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("datetime")]
        public DateTime Datetime { get; set; }
        
        [Column("weight")]
        public int WeightKg { get; set; }
    }
}
