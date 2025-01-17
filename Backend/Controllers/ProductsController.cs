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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReturnedProduct>))]
        public async Task<IActionResult> GetProducts()
        {
            List<ReturnedProduct> products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnedProduct))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                ReturnedProduct product = await _productService.GetOneProduct(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnedProduct))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] NewProduct newProduct)
        {
            try
            {
                ReturnedProduct addedProduct = await _productService.AddProduct(newProduct);
                return Created($"/products/{addedProduct.Id}", addedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
