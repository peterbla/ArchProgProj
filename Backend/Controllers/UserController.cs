using Backend.DatabaseModels;
using Backend.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(UserService userService, TokenService tokenService) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly TokenService _tokenService = tokenService;

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserWithToken))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            try
            {
                var userWithToken = await _userService.Login(userCredentials);
                return Ok(userWithToken);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> Register([FromBody] NewUser newUser)
        {
            try
            {
                await _userService.AddUser(newUser);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
