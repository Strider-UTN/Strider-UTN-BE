using Microsoft.IdentityModel.Tokens;
using StriderWebApi.Data;
using StriderWebApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StriderWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly StriderDbContext _dbContext;

        public AuthService(IConfiguration config, StriderDbContext dbContext)
        {
           _config = config ?? throw new ArgumentNullException(nameof(config));
           _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<string> HandleLogin(string? username, string? password)
        {
            if (username != "admin" || password != "1234") //TODO: Replace with real user validation logic from db
                throw new UnauthorizedAccessException("Invalid username or password.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
