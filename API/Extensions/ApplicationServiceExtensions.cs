namespace API.Extensions;

using System.Text;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

/// <summary>
/// Extensiones para configurar servicios de la aplicación
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registra todos los servicios de aplicación y repositorios
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Entity Framework Core DbContext
        services.AddDbContext<DevManagerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositorios EF Core - IAM
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Repositorios EF Core - Talent
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IEmployeeSkillRepository, EmployeeSkillRepository>();
        
        // Repositorios EF Core - Projects
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();

        // Servicios de aplicación - IAM
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        
        // Servicios de aplicación - Talent
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IEmployeeSkillService, EmployeeSkillService>();
        
        // Servicios de aplicación - Projects
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IAssignmentService, AssignmentService>();

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("DevManagerPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Configura autenticación JWT
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new ArgumentNullException("JwtSettings:SecretKey no configurado");
        var issuer = configuration["JwtSettings:Issuer"] ?? "DevManagerAPI";
        var audience = configuration["JwtSettings:Audience"] ?? "DevManagerClient";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Logging para debugging
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"OnChallenge error: {context.Error}, {context.ErrorDescription}");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Configura Swagger con soporte para JWT
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DevManager API",
                Version = "v1",
                Description = "API para gestión de talento y proyectos con multi-tenancy",
                Contact = new OpenApiContact
                {
                    Name = "DevManager Team",
                    Email = "support@devmanager.com"
                }
            });

            // Configuración de seguridad JWT en Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese 'Bearer' [espacio] y luego su token JWT.\r\n\r\n" +
                              "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        return services;
    }
}