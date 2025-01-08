namespace Backend.DatabaseModels
{
    public class ProductInMeal
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MealEntryId { get; set; }
        public int AmountG { get; set; }

    }
}
