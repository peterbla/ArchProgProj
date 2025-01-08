using Backend.DatabaseModels;
using Backend.Services;

namespace Backend
{
    public static class MockDatabase
    {
        // Do przepisania w przyszłości
        public static User GetUserFromHttpContext(HttpContext httpContext)
        {
            const string userNameClaimsType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            string? userName = httpContext.User.Claims.Where(c => c.Type == userNameClaimsType).Select(c => c.Value).SingleOrDefault();

            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            return users.Find(user => user.Name == userName);
        }

        public static readonly List<User> users =
        [
            new User()
            {
                Id = 1,
                Name = "Test",
                PasswordHash = UserService.HashPassword("password"),
                HeightCm = 132,
                WeightKg = 60,
            },
        ];

        public static readonly List<WeightHistory> weightHistory1 =
        [
            new WeightHistory()
            {
                Id = 1,
                UserId = 1,
                WeightKg = 60,
                Datetime = DateTime.UtcNow
            }
        ];

        public static readonly List<Product> products =
        [
            new Product()
            {
                Id = 1,
                Name = "Nutella",
                Energy = 539,
                Sugars = 56.3f,
            },
            new Product()
            {
                Id = 2,
                Name = "Bułka",
                Energy = 310,

            }
        ];

        public static readonly List<MealEntry> eatingHistory =
        [
            new MealEntry() {
                Id = 1,
                UserId = 1,
                Date = DateTime.UtcNow,
                MealType = Models.MealType.Dinner,
            }
        ];
        public static readonly List<ProductInMeal> productsInMeals =
        [
            new ProductInMeal() {
                Id = 1,
                ProductId = 1,
                MealEntryId = 1,
                AmountG = 30
            },
            new ProductInMeal() {
                Id = 2,
                ProductId = 2,
                MealEntryId = 1,
                AmountG = 80
            }
        ];
    }
}
