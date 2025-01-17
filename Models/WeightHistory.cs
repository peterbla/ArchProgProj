using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NewWeightHistory
    {
        public DateTime Datetime { get; set; }
        public float WeightKg { get; set; }

    }

    public class ReturnedWeightHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Datetime { get; set; }
        public float WeightKg { get; set; }
    }
}
