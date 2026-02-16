using Npgsql;

namespace Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task<Guid> CreateRefreshTokenAsync(RefreshToken token);

    Task<Guid> CreateRefreshTokenAsync(RefreshToken token, NpgsqlConnection connection, NpgsqlTransaction transaction);

    Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash, string token);

    Task RevokeTokenAsync(Guid tokenId);

    Task RevokeTokenAsync(Guid tokenId, NpgsqlConnection connection, NpgsqlTransaction transaction);
}
