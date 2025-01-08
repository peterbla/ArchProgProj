using Models;

namespace Backend.DatabaseModels
{
    public class MealEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public MealType MealType { get; set; }

    }
}
