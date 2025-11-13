using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StriderWebApi.Data;
using StriderWebApi.Data.Repositories;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Services;
using StriderWebApi.Services.Interfaces;
using System.Text;

namespace StriderWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // Ruta del hub
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hub/notifications")) // <- debe coincidir con el MapHub
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            // SignalR
            services.AddSignalR();

            // DbContext
            services.AddDbContext<StriderDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("StriderConnectionString")));

            // Services
            // Helpers
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpContextAccessor();

            // Service Registrations
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAthleteService, AthleteService>();
            services.AddScoped<ICoachService, CoachService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITrainingTemplateService, TrainingTemplateService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ITrainingSessionService, TrainingSessionService>();
            services.AddScoped<ICoachAthleteRelationshipService, CoachAthleteRelationshipService>();
            services.AddScoped<ITrainingGroupService, TrainingGroupService>();
            services.AddScoped<IPlanningService, PlanningService>();
            services.AddScoped<IMesocycleService, MesocycleService>();
            services.AddScoped<IMicrocycleService, MicrocycleService>();
            services.AddScoped<IPeriodService, PeriodService>();

            // Repositories Registrations
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAthleteRepository, AthleteRepository>();
            services.AddScoped<ICoachRepository, CoachRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITrainingTemplateRepository, TrainingTemplateRepository>();
            services.AddScoped<ITrainingSessionsRepository, TrainingSessionRepository>();
            services.AddScoped<ICoachAthleteRelationshipRepository, CoachAthleteRelationshipRepository>();
            services.AddScoped<ITrainingGroupRepository, TrainingGroupRepository>();
            services.AddScoped<IPlanningRepository, PlanningRepository>();
            services.AddScoped<IPlanningAthleteRepository, PlanningAthleteRepository>();
            services.AddScoped<IMesocycleRepository, MesocycleRepository>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IMicrocycleRepository, MicrocycleRepository>();
            services.AddScoped<ITrainingIntervalRepository, TrainingIntervalRepository>();
            services.AddScoped<ITrainingSessionAthleteRepository, TrainingSessionAthleteRepository>();
            services.AddScoped<ITrainingSeriesRepository, TrainingSeriesRepository>();
            services.AddScoped<IAthleteInjuryRepository, AthleteInjuryRepository>();
            services.AddScoped<IAthleteInjuryService, AthleteInjuryService>();
            services.AddScoped<ICompletedWorkoutRepository, CompletedWorkoutRepository>();
            services.AddScoped<ICompletedWorkoutService, CompletedWorkoutService>();

            return services;
        }
    }
}
