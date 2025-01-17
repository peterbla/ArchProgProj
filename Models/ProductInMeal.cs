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
        public string ProductName {  get; set; }
        // Wyliczone dla odpowiedniej ilości gram
        public int Energy { get; set; }
        public float? Fat { get; set; }
        public float? Saturates { get; set; }
        public float? Carbohydrate { get; set; }
        public float? Sugars { get; set; }
        public float? Fibre { get; set; }
        public float? Protein { get; set; }
        public float? Salt { get; set; }

    }
}
