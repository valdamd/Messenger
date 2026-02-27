using Npgsql;

namespace Identity.Api.Jobs;

public sealed class RefreshTokenCleanupJob(
    NpgsqlDataSource dataSource,
    TimeProvider timeProvider,
    ILogger<RefreshTokenCleanupJob> logger)
{
    public async Task CleanupExpiredTokensAsync()
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM refresh_tokens WHERE expires_at_utc < @now";
        command.Parameters.AddWithValue("now", timeProvider.GetUtcNow());

        var deleted = await command.ExecuteNonQueryAsync();
        logger.LogInformation("Hangfire cleanup removed {DeletedCount} expired refresh tokens", deleted);
    }
}
