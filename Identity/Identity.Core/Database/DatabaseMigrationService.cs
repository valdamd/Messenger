using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.Core.Database;

internal sealed class DatabaseMigrationService(
    NpgsqlDataSource dataSource,
    ILogger<DatabaseMigrationService> logger) : BackgroundService
{
    private const int MaxRetries = 10;
    private const int InitialDelaySeconds = 2;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migrations");

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await using var connection = await dataSource.OpenConnectionAsync(stoppingToken);

                foreach (var migration in DatabaseMigrations.GetAllMigrations())
                {
                    await connection.ExecuteAsync(migration);
                }

                logger.LogInformation("Database migrations completed successfully");
                return;
            }
            catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(ex, "Database migrations cancelled");
                return;
            }
            catch (NpgsqlException ex) when (attempt < MaxRetries && !stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(InitialDelaySeconds * attempt);
                logger.LogWarning(
                    ex,
                    "Database connection attempt {Attempt}/{MaxRetries} failed. Retrying in {DelaySeconds} seconds.",
                    attempt,
                    MaxRetries,
                    delay.TotalSeconds);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException ex2)
                {
                    logger.LogInformation(ex2, "Database migrations cancelled");
                }
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Database migration failed");
                throw;
            }
        }

        logger.LogError("Failed to connect to database after {MaxRetries} attempts", MaxRetries);
        throw new InvalidOperationException($"Failed to connect to database after {MaxRetries} attempts");
    }
}
