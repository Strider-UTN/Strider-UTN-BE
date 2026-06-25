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
    public class AuthService(IConfiguration config, IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IAthleteRepository athleteRepository, ICoachRepository coachRepository, IEmailService emailService) : IAuthService
    {
        private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IAthleteRepository _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
        private readonly ICoachRepository _coachRepository = coachRepository ?? throw new ArgumentNullException(nameof(coachRepository));
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        public IPasswordHasher<User> PasswordHasher => _passwordHasher;

        public async Task<string> HandleGoogleLoginAsync(GoogleJsonWebSignature.Payload payload, UserTypeEnum userType)
        {
            var dbUser = await _userRepository.GetUserByEmailAsync(payload.Email, userType);

            if (dbUser == null)
                dbUser = await CreateNewUserFromGooglePayload(payload, userType);

            return GetToken(dbUser, dbUser.FullName);
        }

        private async Task<User?> CreateNewUserFromGooglePayload(GoogleJsonWebSignature.Payload payload, UserTypeEnum userType)
        {
            if (userType == UserTypeEnum.Athlete)
            {
                // Establecer TrainingStartDate al mes y año actual con día 1
                var today = DateTime.UtcNow;
                var trainingStartDate = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

                var newUser = new Athlete
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    Address = "Not provided", // Default address for Google users
                    Gender = Gender.MALE, // Default gender for Google users
                    BirthDate = DateTime.UtcNow.AddYears(-18), // Default birth date for Google users
                    UserType = userType,
                    Active = true, // Assuming Google users are automatically active
                    CreatedBy = "Google SSO",
                    CreatedDate = DateTime.UtcNow,
                    TrainingStartDate = trainingStartDate,
                    YearsOfExperience = 0 // Default to 0 for Google users
                };

                await _athleteRepository.AddAthleteAsync(newUser);
                return newUser;
            }
            else
            {
                var newUser = new Coach
                {
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

            // Si el usuario no existe, lanzar excepción de no autorizado
            if (dbUser == null)
                throw new UnauthorizedAccessException("Email o contraseña inválidos.");

            var isGoogleRegistered = dbUser.CreatedBy == "Google SSO";

            if (isGoogleRegistered)
                throw new UnauthorizedAccessException("Registro con google detectado. Por favor, ingresa seleccionando la opción de 'Iniciar sesión con Google'");

            var passwordIsValid = !string.IsNullOrEmpty(dbUser.PasswordHash) && PasswordHasher.VerifyHashedPassword(dbUser, dbUser.PasswordHash, password) == PasswordVerificationResult.Success;

            if (!passwordIsValid)
                throw new UnauthorizedAccessException("Email o contraseña inválidos.");

            return GetToken(dbUser, dbUser.FullName);
        }

        public async Task ForgotPasswordAsync(string email, UserTypeEnum userType)
        {
            var user = await _userRepository.GetUserByEmailAsync(email, userType)
                ?? throw new KeyNotFoundException("No se encontró un usuario con ese email y tipo.");

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(24);
            user.UpdatedBy = "Password Reset Request";
            user.UpdatedDate = DateTime.UtcNow;

            await UpdateUserAsync(user);
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, token);
        }

        public async Task ResetPasswordAsync(string resetToken, string newPassword)
        {
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(resetToken);
            if (user == null || string.IsNullOrEmpty(user.PasswordResetToken))
                throw new ArgumentException("Token de recuperación inválido.");

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            user.UpdatedBy = "Password Reset";
            user.UpdatedDate = DateTime.UtcNow;

            await UpdateUserAsync(user);
        }

        private async Task UpdateUserAsync(User user)
        {
            if (user.UserType == UserTypeEnum.Athlete && user is Athlete athlete)
            {
                await _athleteRepository.UpdateAthleteAsync(athlete);
                return;
            }

            if (user.UserType == UserTypeEnum.Coach && user is Coach coach)
            {
                await _coachRepository.UpdateCoachAsync(coach);
                return;
            }

            await _userRepository.UpdateUserAsync(user);
        }

        private string GetToken(User user, string name)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Name", name),
                new Claim("Role", user.UserType.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("theme", user.PreferredTheme.ToString().ToLowerInvariant())
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
