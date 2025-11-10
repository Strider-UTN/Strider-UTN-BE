using Microsoft.AspNetCore.Identity;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.UserCreation;
using StriderWebApi.Exceptions.AccountActivation;
using StriderWebApi.Exceptions.User;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class UserService(IUserRepository userRepository, IAthleteRepository athleteRepository, ICoachRepository coachRepository, IPasswordHasher<User> passwordHasher, IJwtService jwtService, IEmailService emailService, ILogger<UserService> logger) : IUserService
    {
        public async Task ActivateAccountAsync(ActivateAccountDto dto)
        {
            var userToActivate = await userRepository.GetUserByIdAsync(dto.UserId) ?? throw new UserNotFoundException("No pudimos encontrar un usuario con el Id indicado.");

            // Check if the activation token is valid and not expired
            if (userToActivate.ActivationToken != dto.ActivationToken || userToActivate.ActivationTokenExpires < DateTime.UtcNow)
            {
                throw new ActivationTokenInvalidOrExpiredException("El token de activación es inválido o ha expirado.");
            }

            userToActivate.Active = true; // Activate the user account
            userToActivate.ActivationToken = null; // Clear the activation token
            userToActivate.ActivationTokenExpires = null; // Clear the expiration date
            userToActivate.UpdatedBy = "Account Activation";
            userToActivate.UpdatedDate = DateTime.UtcNow;

            // Save the updated user to the repository
            await userRepository.UpdateUserAsync(userToActivate);
        }

        public async Task CreateAthleteAsync(CreateAthleteDto dto)
        {
            // Validate if user already exists based on username or email
            await ValidateUserUniquenessAsync(dto.Email, dto.Username, UserTypeEnum.Athlete);

            // Create a new athlete instance
            var newAthlete = new Athlete
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                Height = dto.HeightCm,
                Weight = dto.WeightKg,
                Country = dto.Country,
                Active = true, // Default to false, until account is verified
                CreatedBy = "Athlete Creation",
                CreatedDate = DateTime.UtcNow,
                ActivationToken = Guid.NewGuid().ToString(),
                ActivationTokenExpires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                EmergencyContactRelationship = dto.EmergencyContactRelationship,
                YearsOfExperience = dto.YearsOfExperience,
                TrainingVolumeType = dto.VolumeType,
                TrainingVolumeKm = dto.TrainingVolumeKm,
            };

            // Hash the password
            newAthlete.PasswordHash = passwordHasher.HashPassword(newAthlete, dto.Password);

            // Save the new athlete to the repository
            await athleteRepository.AddAthleteAsync(newAthlete);

            // Add email notification for account activation
            //await _emailService.SendAccountActivationEmailAsync(newAthlete.Email, newAthlete.Username, newAthlete.ActivationToken);
        }
        public async Task CreateCoachAsync(CreateCoachDto dto)
        {
            // Validate if user already exists based on username or email
            await ValidateUserUniquenessAsync(dto.Email, dto.Username, UserTypeEnum.Coach);

            // Create a new coach instance
            var newCoach = new Coach
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                Active = true, // Default to false, until account is verified
                CreatedBy = "Coach Creation",
                CreatedDate = DateTime.UtcNow,
                ActivationToken = Guid.NewGuid().ToString(),
                ActivationTokenExpires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
            };

            // Hash the password
            newCoach.PasswordHash = passwordHasher.HashPassword(newCoach, dto.Password);

            // Save the new athlete to the repository
            await coachRepository.AddCoachAsync(newCoach);

            // Send email notification for account activation
            //await _emailService.SendAccountActivationEmailAsync(newCoach.Email, newCoach.Username, newCoach.ActivationToken);
        }

        private async Task ValidateUserUniquenessAsync(string email, string username, UserTypeEnum userType)
        {
            if (await userRepository.UserExistsByEmailAsync(email, userType))
            {
                throw new UserAlreadyExistsException($"Ya existe un usuario {userType} con el email indicado");
            }

            if (await userRepository.UserExistsByUsernameAsync(username, userType))
            {
                throw new UserAlreadyExistsException($"Ya existe un usuario {userType} con el nombre de usuario indicado");
            }
        }

        public async Task<bool> UpdateUserThemeAsync(int userId, ThemePreference theme)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    logger.LogWarning("Usuario con ID {UserId} no encontrado", userId);
                    return false;
                }

                user.PreferredTheme = theme;
                user.UpdatedBy = jwtService.GetCurrentUserName();
                user.UpdatedDate = DateTime.UtcNow;

                return await userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar el tema del usuario {UserId}", userId);
                throw;
            }
        }
    }
}
