using Backend.DatabaseModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "user")]
    public class MealsController : ControllerBase
    {
        // GET: api/meals
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MealEntry>))]
        public async Task<IActionResult> GetMeals()
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);
            return Ok(MockDatabase.eatingHistory);
        }

        // GET: api/meals/{mealId}
        [HttpGet("{mealId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealEntry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMealById(int mealId)
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);
            MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);

            if (mealEntry == null)
            {
                return NotFound();
            }

            return Ok(mealEntry);
        }

        // POST: api/meals
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MealEntry))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddMeal([FromBody] NewMealEntry eating)
        {
            MealEntry mealEntry = new()
            {
                Id = MockDatabase.eatingHistory.Count + 1,
                UserId = MockDatabase.GetUserFromHttpContext(HttpContext).Id,
                Date = eating.Date,
                MealType = eating.MealType
            };

            MockDatabase.eatingHistory.Add(mealEntry);

            return CreatedAtAction(nameof(GetMealById), new { mealId = mealEntry.Id }, mealEntry);
        }

        // GET: api/meals/{mealId}/products
        [HttpGet("{mealId}/products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductInMeal>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMealProducts(int mealId)
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);
            MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);

            if (mealEntry == null)
            {
                return NotFound();
            }

            return Ok(MockDatabase.productsInMeals);
        }

        // POST: api/meals/{mealId}/products
        [HttpPost("{mealId}/products")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductInMeal))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddProductToMeal(int mealId, [FromBody] NewProductInMeal newProductInMeal)
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);
            MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);

            if (mealEntry == null)
            {
                return NotFound();
            }

            ProductInMeal productInMeal = new()
            {
                Id = MockDatabase.productsInMeals.Count + 1,
                ProductId = newProductInMeal.ProductId,
                MealEntryId = mealId,
                AmountG = newProductInMeal.AmountG,
            };

            MockDatabase.productsInMeals.Add(productInMeal);

            return CreatedAtAction(nameof(GetMealProducts), new { mealId = mealId }, productInMeal);
        }
    }

}
