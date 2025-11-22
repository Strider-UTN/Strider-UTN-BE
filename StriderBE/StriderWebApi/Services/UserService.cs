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
            await ValidateUserUniquenessAsync(dto.Email, UserTypeEnum.Athlete);

            // Parsear TrainingStartDate desde formato YYYY-MM
            DateTime? trainingStartDate = null;
            int yearsOfExperience = 0;
            if (!string.IsNullOrWhiteSpace(dto.TrainingStartDate))
            {
                // Parsear formato YYYY-MM y crear fecha con día 1 (UTC)
                if (DateTime.TryParseExact(dto.TrainingStartDate + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var tsd))
                {
                    if (tsd.Kind != DateTimeKind.Utc)
                    {
                        tsd = DateTime.SpecifyKind(tsd, DateTimeKind.Utc);
                    }
                    trainingStartDate = tsd;
                    // Calcular años de experiencia automáticamente
                    var today = DateTime.UtcNow;
                    yearsOfExperience = today.Year - tsd.Year;
                    if (today.Month < tsd.Month || (today.Month == tsd.Month && today.Day < tsd.Day))
                    {
                        yearsOfExperience--;
                    }
                    yearsOfExperience = Math.Max(0, yearsOfExperience);
                }
            }
            else
            {
                // Si no se proporciona TrainingStartDate, usar mes y año actual
                var today = DateTime.UtcNow;
                trainingStartDate = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                yearsOfExperience = 0;
            }

            // Normalizar BirthDate a UTC si viene Unspecified/Local
            var normalizedBirthDate = dto.BirthDate;
            if (normalizedBirthDate.Kind != DateTimeKind.Utc)
            {
                normalizedBirthDate = DateTime.SpecifyKind(normalizedBirthDate, DateTimeKind.Utc);
            }

            // Create a new athlete instance
            var newAthlete = new Athlete
            {
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = normalizedBirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber ?? string.Empty,
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
                YearsOfExperience = yearsOfExperience,
                TrainingStartDate = trainingStartDate,
                TrainingVolumeType = dto.VolumeType,
                TrainingVolumeKm = dto.TrainingVolumeKm,
                // Información médica
                HasHealthInsurance = dto.HasHealthInsurance,
                HealthInsuranceProvider = dto.HealthInsuranceProvider ?? string.Empty,
                HealthInsuranceMemberNumber = dto.HealthInsuranceMemberNumber ?? string.Empty,
                MedicalConditions = dto.MedicalConditions ?? new List<string>(),
            };

            // Manejar LastCheckupDate y calcular MedicalClearanceExpiryDate
            if (dto.LastCheckupDate.HasValue)
            {
                var lastCheckup = dto.LastCheckupDate.Value;
                if (lastCheckup.Kind != DateTimeKind.Utc)
                {
                    lastCheckup = DateTime.SpecifyKind(lastCheckup, DateTimeKind.Utc);
                }
                newAthlete.LastCheckupDate = lastCheckup;
                // Calcular automáticamente la fecha de expiración (1 año después)
                newAthlete.MedicalClearanceExpiryDate = lastCheckup.AddYears(1);
            }

            // Hash the password
            newAthlete.PasswordHash = passwordHasher.HashPassword(newAthlete, dto.Password);

            // Save the new athlete to the repository
            await athleteRepository.AddAthleteAsync(newAthlete);
        }
        public async Task CreateCoachAsync(CreateCoachDto dto)
        {
            // Validate if user already exists based on username or email
            await ValidateUserUniquenessAsync(dto.Email, UserTypeEnum.Coach);

            // Normalizar BirthDate a UTC
            var normalizedBirthDate = dto.BirthDate;
            if (normalizedBirthDate.Kind != DateTimeKind.Utc)
            {
                normalizedBirthDate = DateTime.SpecifyKind(normalizedBirthDate, DateTimeKind.Utc);
            }

            // Create a new coach instance
                var newCoach = new Coach
                {
                    FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = normalizedBirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
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
        }

        private async Task ValidateUserUniquenessAsync(string email, UserTypeEnum userType)
        {
            if (await userRepository.UserExistsByEmailAsync(email, userType))
            {
                throw new UserAlreadyExistsException($"Ya existe un usuario {userType} con el email indicado");
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

                // Guardar cambios usando el repositorio específico según el tipo
                if (user.UserType == UserTypeEnum.Athlete && user is Athlete athlete)
                {
                    await athleteRepository.UpdateAthleteAsync(athlete);
                }
                else if (user.UserType == UserTypeEnum.Coach && user is Coach coach)
                {
                    await coachRepository.UpdateCoachAsync(coach);
                }
                else
                {
                    return await userRepository.UpdateUserAsync(user);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar el tema del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    logger.LogWarning("Usuario con ID {UserId} no encontrado", userId);
                    return false;
                }

                // Actualizar campos comunes
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    user.FullName = dto.FullName;
                }

                if (dto.PhoneNumber != null)
                {
                    user.PhoneNumber = dto.PhoneNumber;
                }

                if (!string.IsNullOrWhiteSpace(dto.Address))
                {
                    user.Address = dto.Address;
                }

                if (dto.ProfilePictureUrl != null)
                {
                    user.ProfilePictureUrl = dto.ProfilePictureUrl;
                }

                if (dto.BirthDate.HasValue)
                {
                    var bd = dto.BirthDate.Value;
                    if (bd.Kind != DateTimeKind.Utc)
                    {
                        bd = DateTime.SpecifyKind(bd, DateTimeKind.Utc);
                    }
                    user.BirthDate = bd;
                }

                if (dto.Gender.HasValue)
                {
                    user.Gender = dto.Gender.Value;
                }

                if (!string.IsNullOrWhiteSpace(dto.Bio))
                {
                    user.Bio = dto.Bio;
                }

                user.UpdatedBy = jwtService.GetCurrentUserName();
                user.UpdatedDate = DateTime.UtcNow;

                // Actualizar campos específicos de atletas SOLO si es atleta
                if (user.UserType == UserTypeEnum.Athlete && user is Athlete athlete)
                {
                    if (dto.Height.HasValue)
                    {
                        athlete.Height = dto.Height.Value;
                    }

                    if (dto.Weight.HasValue)
                    {
                        athlete.Weight = dto.Weight.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName))
                    {
                        athlete.EmergencyContactName = dto.EmergencyContactName;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.EmergencyContactPhone))
                    {
                        athlete.EmergencyContactPhone = dto.EmergencyContactPhone;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.EmergencyContactRelationship))
                    {
                        athlete.EmergencyContactRelationship = dto.EmergencyContactRelationship;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.Country))
                    {
                        athlete.Country = dto.Country;
                    }

                    // Permitir actualizar VO2Max incluso si viene como null (para limpiar el campo)
                    if (dto.VO2Max != null)
                    {
                        athlete.VO2Max = string.IsNullOrWhiteSpace(dto.VO2Max) ? null : dto.VO2Max;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.TrainingStartDate))
                    {
                        // Parsear formato YYYY-MM y crear fecha con día 1 (UTC)
                        if (DateTime.TryParseExact(dto.TrainingStartDate + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var tsd))
                        {
                            if (tsd.Kind != DateTimeKind.Utc)
                            {
                                tsd = DateTime.SpecifyKind(tsd, DateTimeKind.Utc);
                            }
                            athlete.TrainingStartDate = tsd;
                            // Calcular años de experiencia automáticamente
                            var today = DateTime.UtcNow;
                            var yearsOfExperience = today.Year - tsd.Year;
                            if (today.Month < tsd.Month || (today.Month == tsd.Month && today.Day < tsd.Day))
                            {
                                yearsOfExperience--;
                            }
                            athlete.YearsOfExperience = Math.Max(0, yearsOfExperience);
                        }
                    }

                    if (dto.TrainingVolumeType.HasValue)
                    {
                        athlete.TrainingVolumeType = dto.TrainingVolumeType.Value;
                    }

                    if (dto.TrainingVolumeKm.HasValue)
                    {
                        athlete.TrainingVolumeKm = dto.TrainingVolumeKm.Value;
                    }

                    // Información médica
                    if (dto.HasHealthInsurance.HasValue)
                    {
                        athlete.HasHealthInsurance = dto.HasHealthInsurance.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.HealthInsuranceProvider))
                    {
                        athlete.HealthInsuranceProvider = dto.HealthInsuranceProvider;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.HealthInsuranceMemberNumber))
                    {
                        athlete.HealthInsuranceMemberNumber = dto.HealthInsuranceMemberNumber;
                    }

                    if (dto.LastCheckupDate.HasValue)
                    {
                        athlete.LastCheckupDate = dto.LastCheckupDate.Value;
                        // Calcular automáticamente la fecha de expiración (1 año después)
                        athlete.MedicalClearanceExpiryDate = dto.LastCheckupDate.Value.AddYears(1);
                    }
                    else if (dto.MedicalClearanceExpiryDate.HasValue)
                    {
                        // Permitir actualizar manualmente la fecha de expiración si se envía
                        athlete.MedicalClearanceExpiryDate = dto.MedicalClearanceExpiryDate.Value;
                    }

                    if (dto.MedicalConditions != null)
                    {
                        // Filtrar condiciones vacías y actualizar la lista
                        athlete.MedicalConditions = dto.MedicalConditions
                            .Where(c => !string.IsNullOrWhiteSpace(c))
                            .Select(c => c.Trim())
                            .ToList();
                    }

                    // Guardar cambios del atleta usando el repositorio específico
                    await athleteRepository.UpdateAthleteAsync(athlete);
                }
                else if (user.UserType == UserTypeEnum.Coach && user is Coach coach)
                {
                    // Guardar cambios del coach usando el repositorio específico
                    await coachRepository.UpdateCoachAsync(coach);
                }
                else
                {
                    // Guardar cambios del usuario común
                    return await userRepository.UpdateUserAsync(user);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar el perfil del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<UserProfileResponseDto?> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    logger.LogWarning("Usuario con ID {UserId} no encontrado", userId);
                    return null;
                }

                var response = new UserProfileResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    BirthDate = user.BirthDate,
                    Address = user.Address,
                    Gender = user.Gender,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    UserType = user.UserType,
                    PreferredTheme = user.PreferredTheme,
                    Bio = user.Bio
                };

                // Si es atleta, agregar campos específicos
                if (user.UserType == UserTypeEnum.Athlete && user is Athlete athlete)
                {
                    response.Height = athlete.Height;
                    response.Weight = athlete.Weight;
                    response.EmergencyContactName = athlete.EmergencyContactName;
                    response.EmergencyContactPhone = athlete.EmergencyContactPhone;
                    response.EmergencyContactRelationship = athlete.EmergencyContactRelationship;
                    response.Country = athlete.Country;
                    response.VO2Max = athlete.VO2Max;
                    response.YearsOfExperience = athlete.YearsOfExperience;
                    response.TrainingStartDate = athlete.TrainingStartDate?.ToString("yyyy-MM");
                    response.TrainingVolumeType = athlete.TrainingVolumeType;
                    response.TrainingVolumeKm = athlete.TrainingVolumeKm;
                response.HasHealthInsurance = athlete.HasHealthInsurance;
                response.HealthInsuranceProvider = athlete.HealthInsuranceProvider;
                response.HealthInsuranceMemberNumber = athlete.HealthInsuranceMemberNumber;
                response.LastCheckupDate = athlete.LastCheckupDate;
                response.MedicalClearanceExpiryDate = athlete.MedicalClearanceExpiryDate;
                response.MedicalConditions = athlete.MedicalConditions ?? new List<string>();
            }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el perfil del usuario {UserId}", userId);
                throw;
            }
        }
    }
}
