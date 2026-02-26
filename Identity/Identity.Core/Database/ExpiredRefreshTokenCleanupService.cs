using Identity.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.Core.Database;

internal sealed class ExpiredRefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    NpgsqlDataSource dataSource,
    ILogger<ExpiredRefreshTokenCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromDays(1);
    private static readonly TimeSpan SchemaCheckInterval = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForSchemaReadyAsync(stoppingToken);
        await CleanupOnceAsync(stoppingToken);

        using var timer = new PeriodicTimer(CleanupInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupOnceAsync(stoppingToken);
        }
    }

    private async Task WaitForSchemaReadyAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT to_regclass('public.refresh_tokens') IS NOT NULL";

                var result = await command.ExecuteScalarAsync(cancellationToken);
                if (result is bool exists && exists)
                {
                    return;
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Waiting for refresh_tokens table before cleanup starts");
            }

            try
            {
                await Task.Delay(SchemaCheckInterval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private async Task CleanupOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
            var cleanupResult = await refreshTokenRepository.DeleteExpiredTokensAsync(timeProvider.GetUtcNow());
            logger.LogInformation(
                "Expired refresh token cleanup completed. Deleted: {DeletedCount}",
                cleanupResult.DeletedTokensCount);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(ex, "Expired refresh token cleanup cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to cleanup expired refresh tokens");
        }
    }
}
