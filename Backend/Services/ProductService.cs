using Backend.DatabaseModels;

using Models;

using Supabase;
using Supabase.Postgrest.Responses;

namespace Backend.Services
{
    public class ProductService(Client supabaseClient)
    {
        private readonly Client _supabaseClient = supabaseClient;

        public async Task<List<ReturnedProduct>> GetAllProducts()
        {
            ModeledResponse<Product> results = await _supabaseClient.From<Product>().Get();
            return results.Models.Select(x => x.ConwertToReturn()).ToList();
        }

        public async Task<ReturnedProduct> GetOneProduct(int id)
        {
            Product? product = await _supabaseClient.From<Product>().Where(x => x.Id == id).Single();
            if (product == null)
            {
                throw new Exception("Product does not exist.");
            }
            return product.ConwertToReturn();
        }

        public async Task<ReturnedProduct> AddProduct(NewProduct newProduct)
        {
            Product product = new()
            {
                Name = newProduct.Name,
                Energy = newProduct.Energy,
                Fat = newProduct.Fat,
                Saturates = newProduct.Saturates,
                Carbohydrate = newProduct.Carbohydrate,
                Sugars = newProduct.Sugars,
                Fibre = newProduct.Fibre,
                Protein = newProduct.Protein,
                Salt = newProduct.Salt,
            };

            ModeledResponse<Product> createdProduct = await _supabaseClient.From<Product>().Insert(product);
            return createdProduct.Model.ConwertToReturn();

        }
    }
}
