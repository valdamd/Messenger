using Microsoft.Extensions.DependencyInjection;
using Pingo.Messeges.Infrastructure.DataBase;

namespace Pingo.Messages.ComponentTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly IServiceScope _scope = factory.Services.CreateScope();

    protected readonly HttpClient HttpClient = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await CleanDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    private async Task CleanDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MessagesDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
