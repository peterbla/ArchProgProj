namespace Backend.DatabaseModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public int? HeightCm { get; set; }
        public int? WeightKg { get; set; }
    }
}
