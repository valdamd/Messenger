using Npgsql;

namespace Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task<Guid> CreateRefreshTokenAsync(RefreshToken token);

    Task<Guid> CreateRefreshTokenAsync(RefreshToken token, NpgsqlConnection connection, NpgsqlTransaction transaction);

    Task<RefreshToken?> ConsumeValidRefreshTokenAsync(
        string tokenHash,
        string token,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction);

    Task RevokeTokenAsync(Guid tokenId);

    Task RevokeTokenAsync(Guid tokenId, NpgsqlConnection connection, NpgsqlTransaction transaction);

    Task<ExpiredTokensCleanupResult> DeleteExpiredTokensAsync(DateTimeOffset now);
}
