using Microsoft.OpenApi.Models;
using StriderWebApi.Extensions;
using StriderWebApi.Helpers;
using StriderWebApi.Hubs;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProjectServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(
            JsonNamingPolicy.CamelCase, // O JsonNamingPolicy.SnakeCaseLower
            allowIntegerValues: true
        ));
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Strider API", Version = "v1" });

    // JWT Support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese su token JWT.\r\n\r\nEjemplo: \"eyJhbGciOi...\""
    });

    // Esto aplica el token a todos los endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var frontendBaseUrl = builder.Configuration["AppSettings:FrontendBaseUrl"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            if (!string.IsNullOrEmpty(frontendBaseUrl))
            {
                policy.WithOrigins(frontendBaseUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
            else
            {
                // Fallback para desarrollo local
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
});

var app = builder.Build();

// Configurar el puerto desde la variable de entorno PORT (para Render, Railway, etc.)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

// Configure the HTTP request pipeline.
// Swagger disponible en desarrollo y producción (útil para testing)
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
else
{
    // En producción, Swagger UI solo con autenticación o deshabilitado
    // Por ahora lo dejamos habilitado para testing, puedes comentarlo después
    app.UseSwaggerUI();
}

// Solo redirigir HTTPS si no estamos en un entorno que maneja HTTPS externamente (como Render)
if (!app.Environment.IsProduction() || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT")))
{
    app.UseHttpsRedirection();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hub/notifications");

app.Run();