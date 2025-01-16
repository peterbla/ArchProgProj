using Backend.DatabaseModels;
using Backend.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "user")]
    public class ProductsController(ProductService productService) : Controller
    {
        private readonly ProductService _productService = productService;

        // GET: api/products
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Product>))]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProductById(int id)
        {
            var product = MockDatabase.products.Find(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateProduct([FromBody] NewProduct newProduct)
        {
            if (newProduct == null)
            {
                return BadRequest("Invalid product data");
            }

            var product = new Product
            {
                Id = MockDatabase.products.Count + 1,
                Name = newProduct.Name,
                Energy = newProduct.Energy,
                Fat = newProduct.Fat,
                Saturates = newProduct.Saturates,
                Carbohydrate = newProduct.Carbohydrate,
                Sugars = newProduct.Sugars,
                Fibre = newProduct.Fibre,
                Protein = newProduct.Protein,
                Salt = newProduct.Salt
            };

            MockDatabase.products.Add(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
    }
}
