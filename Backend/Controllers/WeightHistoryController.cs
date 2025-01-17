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
    public class WeightHistoryController(UserService userService, WeightHistoryService weightHistoryService) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly WeightHistoryService _weightHistoryService = weightHistoryService;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReturnedWeightHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeightHistory()
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                List<ReturnedWeightHistory> weightHistory = await _weightHistoryService.GetUserWeightHistory(user);
                return Ok(weightHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{weightId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnedWeightHistory))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingleWeight(int weightId)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedWeightHistory weight = await _weightHistoryService.GetUserSingleWeight(user, weightId);
                return Ok(weight);
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

        [HttpGet("newest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnedWeightHistory))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNewestWeightHistory()
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedWeightHistory weightHistory = await _weightHistoryService.GetUserNewestWeight(user);
                return Ok(weightHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnedWeightHistory))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddWeightHistory([FromBody] NewWeightHistory newWeightHistory)
        {
            try
            {
                User user = await _userService.GetUserFromHttpContext(HttpContext);
                ReturnedWeightHistory weightHistory = await _weightHistoryService.AddNewUserWeight(user, newWeightHistory);
                return Created($"/weightHistory/{weightHistory.Id}", weightHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
