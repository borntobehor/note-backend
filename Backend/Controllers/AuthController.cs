using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdString, out Guid paseGuid)) return paseGuid;
            throw new UnauthorizedAccessException("Invalid token.");
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> RegisterUser(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            Console.WriteLine(user);
            if (user is null) return BadRequest(new
            {
                result = false,
                message = "Username already exists"
            });
            return Ok(new
            {
                result = true,
                message = "Register successfull",
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {   
            var token = await authService.LoginAsync(request);
            if (token is null) 
                return BadRequest(new
                {
                    result = false,
                    message = "Invalid username or password"
                });

            return Ok(new {
                result = true,
                message = "Login successfull",
                token
            });
        }

        [Authorize]
        [HttpGet]
        [Route("me")]
        public async Task<ActionResult<User>> GetMe(NoteDbContext context)
        {

            var getUser = await context.Users.FindAsync(GetUserId());
            if (getUser is null) 
                return NotFound(new
                {
                    result = false,
                    message = "User not found"
                });
            return Ok(new
            {
                result = true,
                message = "Authenticated",
                id = getUser?.id,
                username = getUser?.username,
            });
        }

    }
}
