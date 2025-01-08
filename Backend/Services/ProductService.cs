using Backend.DatabaseModels;

using Supabase;
using Supabase.Postgrest.Responses;

namespace Backend.Services
{
    public class ProductService
    {
        private readonly Client _supabaseClient;
        public ProductService(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            ModeledResponse<Product>? results = await _supabaseClient.From<Product>().Get();
            return results.Models;
        }
    }
}
