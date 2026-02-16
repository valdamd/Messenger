using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

public sealed class RefreshTokenRepository(NpgsqlDataSource dataSource) : IRefreshTokenRepository
{
    public async Task<Guid> CreateRefreshTokenAsync(RefreshToken token)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var tokenId = await CreateRefreshTokenAsync(token, connection, transaction);
        await transaction.CommitAsync();
        return tokenId;
    }

    public async Task<Guid> CreateRefreshTokenAsync(RefreshToken token, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        const string query = """
                             INSERT INTO refresh_tokens (id, user_id, token, token_hash, expires_at_utc, created_at_utc, is_revoked)
                             VALUES (@Id, @UserId, @Token, @TokenHash, @ExpiresAtUtc, @CreatedAtUtc, @IsRevoked)
                             """;

        await connection.ExecuteAsync(query, token, transaction);
        return token.Id;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash, string token)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string byHashQuery = """
                                    SELECT id, user_id AS UserId, token, token_hash AS TokenHash, expires_at_utc AS ExpiresAtUtc,
                                           created_at_utc AS CreatedAtUtc, is_revoked AS IsRevoked
                                    FROM refresh_tokens
                                    WHERE token_hash = @TokenHash
                                      AND NOT is_revoked
                                      AND expires_at_utc > NOW()
                                    """;

        var hashedToken = await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            byHashQuery,
            new
            {
                TokenHash = tokenHash,
            });
        if (hashedToken is not null)
        {
            return hashedToken;
        }

        const string legacyQuery = """
                                   SELECT id, user_id AS UserId, token, token_hash AS TokenHash, expires_at_utc AS ExpiresAtUtc,
                                          created_at_utc AS CreatedAtUtc, is_revoked AS IsRevoked
                                   FROM refresh_tokens
                                   WHERE token = @Token
                                     AND NOT is_revoked
                                     AND expires_at_utc > NOW()
                                   """;

        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            legacyQuery,
            new
            {
                Token = token,
            });
    }

    public async Task RevokeTokenAsync(Guid tokenId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        await RevokeTokenAsync(tokenId, connection, transaction);
        await transaction.CommitAsync();
    }

    public async Task RevokeTokenAsync(Guid tokenId, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        const string query = """
                             UPDATE refresh_tokens
                             SET is_revoked = TRUE
                             WHERE id = @TokenId
                             """;

        await connection.ExecuteAsync(query, new
        {
            TokenId = tokenId,
        }, transaction);
    }
}
