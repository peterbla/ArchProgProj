using System.Security.Cryptography;
using System.Text;

using Backend.DatabaseModels;

using Supabase;

namespace Backend.Services
{
    public class UserService
    {
        private readonly Client _supabaseClient;
        public UserService(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
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

        public User GetUserFromHttpContext(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
