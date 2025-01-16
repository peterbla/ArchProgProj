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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserCredentialsWithToken))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            User? expectedUser = MockDatabase.users.Find(u => u.Name == userCredentials.Name);

            if (expectedUser == null)
            {
                return NotFound("Invalid username or password");
            }

            if (expectedUser.PasswordHash != UserService.HashPassword(userCredentials.Password))
            {
                return NotFound("Invalid username or password");
            }

            var token = "Bearer " + _tokenService.GenerateToken(expectedUser);
            userCredentials.Password = String.Empty;

            return Ok(new UserCredentialsWithToken { User = userCredentials, Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] NewUser newUser)
        {
            throw new NotImplementedException();
        }
    }
}
