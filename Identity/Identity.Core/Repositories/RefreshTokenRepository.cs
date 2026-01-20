using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

public sealed class RefreshTokenRepository(NpgsqlDataSource dataSource) : IRefreshTokenRepository
{
    public async Task<Guid> CreateRefreshTokenAsync(RefreshToken token)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
                             INSERT INTO refresh_tokens (id, user_id, token, expires_at_utc, created_at_utc, is_revoked)
                             VALUES (@Id, @UserId, @Token, @ExpiresAtUtc, @CreatedAtUtc, @IsRevoked)
                             """;

        await connection.ExecuteAsync(query, token);
        return token.Id;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
                             SELECT id, user_id AS UserId, token, expires_at_utc AS ExpiresAtUtc,
                                    created_at_utc AS CreatedAtUtc, is_revoked AS IsRevoked
                             FROM refresh_tokens
                             WHERE token = @Token
                               AND NOT is_revoked
                               AND expires_at_utc > NOW()
                             """;

        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            query,
            new
            {
                Token = token,
            });
    }

    public async Task RevokeTokenAsync(Guid tokenId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
                             UPDATE refresh_tokens
                             SET is_revoked = TRUE
                             WHERE id = @TokenId
                             """;

        await connection.ExecuteAsync(query, new
        {
            TokenId = tokenId,
        });
    }
}
