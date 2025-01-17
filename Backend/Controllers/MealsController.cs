using Backend.DatabaseModels;
using Backend.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "user")]
    public class MealsController(MealsService mealsService, UserService userService) : ControllerBase
    {
        private readonly MealsService _mealsService = mealsService;
        private readonly UserService _userService = userService;
        

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReturnedMealEntry>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMeals()
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                List<ReturnedMealEntry> meals = await _mealsService.GetAllUserMeals(user);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{mealId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnedMealEntry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMealById(int mealId)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                var meal = await _mealsService.GetOneUserMeal(user, mealId);
                return Ok(meal);
            }
            catch (ArgumentException ax)
            {
                return BadRequest(ax.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnedMealEntry))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMeal([FromBody] NewMealEntry newMealEntry)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedMealEntry meal = await _mealsService.AddNewMeal(user, newMealEntry);
                return Created($"/meals/{meal.Id}", meal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{mealId}/products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReturnedProductInMeal>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMealProducts(int mealId)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedMealEntry meal = await _mealsService.GetOneUserMeal(user, mealId);
                return Ok(meal.ProductsInMeal);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{mealId}/products")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnedProductInMeal))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddProductToMeal(int mealId, [FromBody] NewProductInMeal newProductInMeal)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedProductInMeal productInMeal = await _mealsService.AddProductToMeal(user, mealId, newProductInMeal);
                return Created("",productInMeal);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
