using System.Text;
using FluentValidation;
using Identity.Api.DTOs.Auth;
using Identity.Api.Middleware;
using Identity.Api.Services;
using Identity.Core.DependencyInjection;
using Identity.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Identity.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<LinkService>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins("http://localhost:7000", "http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var dataSource = dataSourceBuilder.Build();
        builder.Services.AddSingleton(dataSource);

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
        builder.Services.AddIdentityCoreServices();

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>()
                         ?? throw new InvalidOperationException("Jwt configuration section is missing.");
        ValidateJwtOptions(jwtOptions);
        builder.Services.AddSingleton(jwtOptions);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)),
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions
                    .TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddBackgroundServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCoreBackgroundServices();

        return builder;
    }

    private static void ValidateJwtOptions(JwtAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Key) || Encoding.UTF8.GetByteCount(options.Key) < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");
        }

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            throw new InvalidOperationException("Jwt:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException("Jwt:Audience is required.");
        }

        if (options.DurationInMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:DurationInMinutes must be positive.");
        }

        if (options.RefreshTokenExpirationDays <= 0)
        {
            throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays must be positive.");
        }
    }
}
