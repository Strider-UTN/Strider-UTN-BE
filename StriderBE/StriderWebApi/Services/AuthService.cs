using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StriderWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IAthleteRepository _athleteRepository;
        private readonly ICoachRepository _coachRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService(IConfiguration config, IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IAthleteRepository athleteRepository, ICoachRepository coachRepository)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _coachRepository = coachRepository ?? throw new ArgumentNullException(nameof(coachRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<string> HandleGoogleLoginAsync(GoogleJsonWebSignature.Payload payload, UserTypeEnum userType)
        {
            var dbUser = await _userRepository.GetUserByEmailAsync(payload.Email, userType);

            if (dbUser == null)
                dbUser = await CreateNewUserFromGooglePayload(payload, userType);

            return GetToken(dbUser.Id, payload.Name, dbUser.UserType);
        }

        private async Task<User?> CreateNewUserFromGooglePayload(GoogleJsonWebSignature.Payload payload, UserTypeEnum userType)
        {
            if (userType == UserTypeEnum.Athlete)
            {
                var newUser = new Athlete
                {
                    Username = payload.Email.Split('@')[0], // Use email prefix as username
                    Email = payload.Email,
                    FullName = payload.Name,
                    Address = "Not provided", // Default address for Google users
                    Gender = Gender.MALE, // Default gender for Google users
                    BirthDate = DateTime.UtcNow.AddYears(-18), // Default birth date for Google users
                    UserType = userType,
                    Active = true, // Assuming Google users are automatically active
                    CreatedBy = "Google SSO",
                    CreatedDate = DateTime.UtcNow,
                };

                await _athleteRepository.AddAthleteAsync(newUser);
                return newUser;
            }
            else
            {
                var newUser = new Coach
                {
                    Username = payload.Email.Split('@')[0], // Use email prefix as username
                    Email = payload.Email,
                    FullName = payload.Name,
                    Address = "Not provided", // Default address for Google users
                    Gender = Gender.MALE, // Default gender for Google users
                    BirthDate = DateTime.UtcNow.AddYears(-18), // Default birth date for Google users
                    UserType = userType,
                    Active = true, // Assuming Google users are automatically active
                    CreatedBy = "Google SSO",
                    CreatedDate = DateTime.UtcNow,
                };
                await _coachRepository.AddCoachAsync(newUser);
                return newUser;
            }
        }

        public async Task<string> HandleLoginAsync(string email, string password, UserTypeEnum userType)
        {
            var dbUser = await _userRepository.GetUserByEmailAsync(email, userType);

            var isGoogleRegistered = dbUser.CreatedBy == "Google SSO";

            if (isGoogleRegistered)
                throw new UnauthorizedAccessException("Registro con google detectado. Por favor, ingresa seleccionando la opción de 'Iniciar sesión con Google'");

            var passwordIsValid = dbUser != null && !string.IsNullOrEmpty(dbUser.PasswordHash) && _passwordHasher.VerifyHashedPassword(dbUser, dbUser.PasswordHash, password) == PasswordVerificationResult.Success;

            if (!passwordIsValid)
                throw new UnauthorizedAccessException("Email o Contraseña inválidos. Por favor, revisa e intenta de nuevo.");

            return GetToken(dbUser.Id, dbUser.Username, dbUser.UserType);
        }

        private string GetToken(int userId, string username, UserTypeEnum type)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("Name", username),
                new Claim("Role", type.ToString())
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
