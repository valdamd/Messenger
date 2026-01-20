using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.Core.Database;

public sealed class DatabaseMigrationService(
    NpgsqlDataSource dataSource,
    ILogger<DatabaseMigrationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("🔄 Начинаю применение миграций...");

            await using var connection = await dataSource.OpenConnectionAsync(stoppingToken);

            foreach (var migration in DatabaseMigrations.GetAllMigrations())
            {
                await connection.ExecuteAsync(migration);
            }

            logger.LogInformation("✅ Миграции успешно применены");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Ошибка при применении миграций");
            throw;
        }
    }
}
