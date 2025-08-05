using Microsoft.AspNetCore.Identity;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.UserCreation;
using StriderWebApi.Exceptions.User;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAthleteRepository _athleteRepository;
        private readonly ICoachRepository _coachRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, IAthleteRepository athleteRepository, ICoachRepository coachRepository, IPasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _userRepository = userRepository;
            _athleteRepository = athleteRepository;
            _coachRepository = coachRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        public async Task CreateAthleteAsync(CreateAthleteDto dto)
        {
            // Validate if user already exists based on username or email
            await ValidateUserUniquenessAsync(dto.Email, dto.Username);

            // Create a new athlete instance
            var newAthlete = new Athlete
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                HeightCm = dto.HeightCm,
                WeightKg = dto.WeightKg,
                Country = dto.Country,
                Active = false, // Default to false, until account is verified
                CreatedBy = "Athlete Creation",
                CreatedDate = DateTime.UtcNow,
                ActivationToken = Guid.NewGuid().ToString(),
                ActivationTokenExpires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
            };

            // Hash the password
            newAthlete.PasswordHash = _passwordHasher.HashPassword(newAthlete, dto.Password);

            // Save the new athlete to the repository
            await _athleteRepository.AddAthleteAsync(newAthlete);

            // Add email notification for account activation
            // Comentado por ahora hasta que se testee la funcionalidad de envío de emails
            //await _emailService.SendAccountActivationEmailAsync(newAthlete.Email, newAthlete.ActivationToken, newAthlete.Username);
        }
        public async Task CreateCoachAsync(CreateCoachDto dto)
        {
            // Validate if user already exists based on username or email
            await ValidateUserUniquenessAsync(dto.Email, dto.Username);

            // Create a new coach instance
            var newCoach = new Coach
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Gender = dto.Gender,
                Active = false, // Default to false, until account is verified
                CreatedBy = "Coach Creation",
                CreatedDate = DateTime.UtcNow,
                ActivationToken = Guid.NewGuid().ToString(),
                ActivationTokenExpires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
            };

            // Hash the password
            newCoach.PasswordHash = _passwordHasher.HashPassword(newCoach, dto.Password);

            // Save the new athlete to the repository
            await _coachRepository.AddCoachAsync(newCoach);

            // Send email notification for account activation
            // Comentado por ahora hasta que se testee la funcionalidad de envío de emails
            //await _emailService.SendAccountActivationEmailAsync(newCoach.Email, newCoach.ActivationToken, newCoach.Username);
        }

        private async Task ValidateUserUniquenessAsync(string email, string username)
        {
            if (await _userRepository.UserExistsByEmailAsync(email))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con el email indicado...");
            }

            if (await _userRepository.UserExistsByUsernameAsync(username))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con el nombre de usuario indicado...");
            }
        }
    }
}
