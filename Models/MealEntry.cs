using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public enum MealType
    {
        None = 0,
        Breakfast = 10,
        Lunch = 20,
        Dinner = 30,
        Supper = 40
    }
    public class NewMealEntry
    {
        public DateOnly Date { get; set; }
        public MealType MealType { get; set; }
    }

    public class ReturnedMealEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
        public MealType MealType { get; set; }
        public List<ReturnedProductInMeal> ProductsInMeal { get; set; }
        public int OverallEnergy { get; set; }
        public float? OverallFat { get; set; }
        public float? OverallSaturates { get; set; }
        public float? OverallCarbohydrate { get; set; }
        public float? OverallSugars { get; set; }
        public float? OverallFibre { get; set; }
        public float? OverallProtein { get; set; }
        public float? OverallSalt { get; set; }
    }
}
