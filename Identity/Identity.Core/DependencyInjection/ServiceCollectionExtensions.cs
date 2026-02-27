using Identity.Core.Database;
using Identity.Core.Repositories;
using Identity.Core.Security;
using Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICredentialsRepository, CredentialsRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    public static IServiceCollection AddIdentityCoreBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseMigrationService>();

        return services;
    }
}
