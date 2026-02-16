using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.Core.Database;

public sealed class DatabaseMigrationService(
    NpgsqlDataSource dataSource,
    ILogger<DatabaseMigrationService> logger) : BackgroundService
{
    private const int MaxRetries = 10;
    private const int InitialDelaySeconds = 2;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Начинаю применение миграций...");

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await using var connection = await dataSource.OpenConnectionAsync(stoppingToken);

                foreach (var migration in DatabaseMigrations.GetAllMigrations())
                {
                    await connection.ExecuteAsync(migration);
                }

                logger.LogInformation("Миграции успешно применены");
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Применение миграций отменено");
                return;
            }
            catch (NpgsqlException ex) when (attempt < MaxRetries && !stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(InitialDelaySeconds * attempt);
                logger.LogWarning(
                    ex,
                    "Попытка подключения к БД {Attempt}/{MaxRetries} не удалась. Повтор через {DelaySeconds} сек.",
                    attempt,
                    MaxRetries,
                    delay.TotalSeconds);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Применение миграций отменено");
                }
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Ошибка при применении миграций");
                throw;
            }
        }

        logger.LogError("Не удалось подключиться к базе данных после {MaxRetries} попыток", MaxRetries);
        throw new InvalidOperationException($"Failed to connect to database after {MaxRetries} attempts");
    }
}
