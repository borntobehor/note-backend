using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Entities;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/aut/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> RegisterUser(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null) return BadRequest("Username already exists");
            return Ok(user);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {   
            var token = await authService.LoginAsync(request);
            if (token is null) return BadRequest("Invalid user name or password");
            return Ok(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authicated");
        }

    }
}
