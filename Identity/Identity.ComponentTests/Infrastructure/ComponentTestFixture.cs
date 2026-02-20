using System.Net.Http.Json;
using Identity.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Respawn;

namespace Identity.ComponentTests.Infrastructure;

public abstract class ComponentTestFixture : IAsyncLifetime
{
    private readonly IdentityWebAppFactory _factory;
    private Respawner? _respawner;

    protected ComponentTestFixture(IdentityWebAppFactory factory)
    {
        _factory = factory;
    }

    public HttpClient CreateClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
    });

    public async Task InitializeAsync()
    {
        await EnsureRespawnerCreatedAsync();
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    private async Task EnsureRespawnerCreatedAsync()
    {
        if (_respawner is not null)
        {
            return;
        }

        // Trigger app startup so DatabaseMigrationService runs migrations
        using var client = CreateClient();
        await WaitForMigrationsAsync();

        await using var connection = new NpgsqlConnection(_factory.ConnectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
        });
    }

    private async Task WaitForMigrationsAsync()
    {
        const int maxAttempts = 30;
        for (var i = 0; i < maxAttempts; i++)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_factory.ConnectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'users'";
                var result = await command.ExecuteScalarAsync();
                if (result is long count && count > 0)
                {
                    return;
                }
            }
            catch
            {
                // DB not ready yet
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException("Database migrations did not complete in time.");
    }

    private async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
        {
            return;
        }

        await using var connection = new NpgsqlConnection(_factory.ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    protected async Task<AccessTokensDto> RegisterUserAsync(
        HttpClient client,
        string email = "test@test.com",
        string name = "Test User",
        string password = "SecurePass123!")
    {
        var dto = new RegisterUserDto
        {
            Email = email,
            Name = name,
            Password = password,
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", dto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
        Assert.NotNull(result);

        return await LoginUserAsync(client, email, password);
    }

    protected async Task<AccessTokensDto> LoginUserAsync(
        HttpClient client,
        string email = "test@test.com",
        string password = "SecurePass123!")
    {
        var dto = new LoginUserDto
        {
            Email = email,
            Password = password,
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", dto);
        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(tokens);

        return tokens;
    }
}
