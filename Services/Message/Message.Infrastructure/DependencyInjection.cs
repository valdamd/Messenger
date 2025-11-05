using Message.Message.Domain;
using Microsoft.EntityFrameworkCore;

namespace Message.Message.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("MessengerDb"));

        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
