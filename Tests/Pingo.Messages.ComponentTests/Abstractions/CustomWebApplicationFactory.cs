using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pingo.Messeges.Infrastructure.DataBase;

namespace Pingo.Messages.ComponentTests.Abstractions;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MessagesDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<MessagesDbContext>();

            services.AddDbContext<MessagesDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MessagesDbContext>();
            db.Database.EnsureCreated();
        });
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
