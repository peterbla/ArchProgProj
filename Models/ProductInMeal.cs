using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NewProductInMeal
    {
        public int ProductId { get; set; }
        public int AmountG { get; set; }
    }

    public class ReturnedProductInMeal
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MealEntryId { get; set; }
        public int AmountG { get; set; }

    }
}
