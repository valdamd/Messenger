using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pingo.Messages.Application;
using Pingo.Messages.Infrastructure.DataBase;
using Pingo.Messages.Infrastructure.Repositories;

namespace Pingo.Messeges.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<MessagesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IMessageRepository, MessageRepository.MessageRepository>();

        return services;
    }
}
