using Backend.DatabaseModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "user")]
    public class WeightHistoryController : ControllerBase
    {
        // GET: api/weightHistory
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WeightHistory>))]
        public IActionResult GetWeightHistory()
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);
            return Ok(MockDatabase.weightHistory1);
        }

        // GET: api/weightHistory/{weightId}
        [HttpGet("{weightId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeightHistory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetSingleWeight(int weightId)
        {
            User user = MockDatabase.GetUserFromHttpContext(HttpContext);

            if (weightId < 0 || weightId >= MockDatabase.weightHistory1.Count)
            {
                return NotFound();
            }

            var weight = MockDatabase.weightHistory1[weightId];
            return Ok(weight);
        }

        // POST: api/weightHistory
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WeightHistory))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddWeightHistory([FromBody] NewWeightHistory newWeightHistory)
        {
            if (newWeightHistory == null)
            {
                return BadRequest("Invalid weight history data.");
            }

            User user = MockDatabase.GetUserFromHttpContext(HttpContext);

            WeightHistory weightHistory = new()
            {
                Id = MockDatabase.weightHistory1.Count + 1,
                UserId = user.Id,
                // Dodaj więcej pól, jeśli są obecne w modelu NewWeightHistory
            };

            MockDatabase.weightHistory1.Add(weightHistory);

            return CreatedAtAction(nameof(GetSingleWeight), new { weightId = weightHistory.Id }, weightHistory);
        }
    }

}
