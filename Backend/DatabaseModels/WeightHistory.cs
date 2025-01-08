namespace Backend.DatabaseModels
{
    public class WeightHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Datetime { get; set; }
        public int WeightKg { get; set; }
    }
}
