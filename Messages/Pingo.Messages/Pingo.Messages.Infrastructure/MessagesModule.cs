using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pingo.Messages.Application;
using Pingo.Messages.Domain;
using Pingo.Messages.Infrastructure.Database;
using Pingo.Messeges.Infrastructure;
using Pingo.Messeges.Infrastructure.DataBase;

namespace Pingo.Messages.Infrastructure;

public static class MessagesModule
{
    public static IServiceCollection AddMessagesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MessagesDb")
                               ?? throw new InvalidOperationException("Connection string 'MessagesDb' not found.");

        services.AddDbContext<MessagesDbContext>(options =>
            options
                .UseNpgsql(
                    connectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Messages))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MessagesDbContext>());
    }
}
