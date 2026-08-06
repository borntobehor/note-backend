using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService(NoteDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(UserDto request)
        {
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.username == request.username);

            if (user is null) return null;
            if (
                new PasswordHasher<User>()
                    .VerifyHashedPassword(user, user.passwordHash, request.password) 
                == PasswordVerificationResult.Failed
                ) return null;
            
            return CreateToken(user);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.username == request.username)) 
                return null;

            var user = new User();

            var hashPassword = new PasswordHasher<User>().HashPassword(user, request.password);

            user.username = request.username;
            user.passwordHash = hashPassword;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
