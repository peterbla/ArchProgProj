using System.Security.Cryptography;
using System.Text;

using Backend.DatabaseModels;

using Models;

using Supabase;
using Supabase.Postgrest.Responses;

namespace Backend.Services
{
    public class UserService(Client supabaseClient, TokenService tokenService)
    {
        private readonly Client _supabaseClient = supabaseClient;
        private readonly TokenService _tokenService = tokenService;

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Zamiana hasła na tablicę bajtów + solenie
                byte[] bytes = Encoding.UTF8.GetBytes(password + "salt");

                // Hashowanie danych
                byte[] hash = sha256.ComputeHash(bytes);

                // Zamiana zhashowanych bajtów na string w formacie hex
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2")); // Formatowanie jako heksadecymalne liczby
                }

                return result.ToString();
            }
        }

        public async Task<User> GetUserFromHttpContext(HttpContext httpContext)
        {
            const string userNameClaimsType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            string? userName = httpContext.User.Claims.Where(c => c.Type == userNameClaimsType).Select(c => c.Value).SingleOrDefault();

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Invalid http context!");
            }

            User? databaseUser = await _supabaseClient.From<User>().Where(x => x.Name == userName).Single();
            if (databaseUser == null)
            {
                throw new ArgumentException("Invalid name!");
            }
            return databaseUser;
        }

        public async Task AddUser(NewUser newUser)
        {
            ModeledResponse<User> result = await _supabaseClient.From<User>().Where(x => x.Name == newUser.Name).Get();
            if (result.Models.Any())
            {
                throw new Exception("User with this username already exists!");
            }
            var user = new User
            {
                Name = newUser.Name,
                PasswordHash = HashPassword(newUser.Password),
                HeightCm = newUser.HeightCm,
                WeightKg = newUser.WeightKg

            };
            await _supabaseClient.From<User>().Insert(user);
        }
        
        public async Task<UserWithToken> Login(UserCredentials userCredentials)
        {
            User? databaseUser = await _supabaseClient.From<User>().Where(x => x.Name == userCredentials.Name).Single();
            if (databaseUser == null)
            {
                throw new Exception("Invalid name or password!");
            }
            if(HashPassword(userCredentials.Password) != databaseUser.PasswordHash)
            {
                throw new Exception("Invalid name or password!");
            }

            NewUser user = new NewUser
            {
                Name = databaseUser.Name,
                Password = String.Empty,
                HeightCm = databaseUser.HeightCm,
                WeightKg = databaseUser.WeightKg
            };

            UserWithToken userWithToken = new UserWithToken
            {
                User = user,
                Token = "Bearer " + _tokenService.GenerateToken(databaseUser)
            };
            return userWithToken;
        }
    }
}
