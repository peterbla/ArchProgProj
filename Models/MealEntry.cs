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
        public DateTime Date { get; set; }
        public MealType MealType { get; set; }
    }
}
