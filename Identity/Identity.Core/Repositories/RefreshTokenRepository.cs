using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

internal sealed class RefreshTokenRepository(NpgsqlDataSource dataSource) : IRefreshTokenRepository
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

    public async Task<RefreshToken?> ConsumeValidRefreshTokenAsync(
        string tokenHash,
        string token,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        const string consumeByHashQuery = """
            UPDATE refresh_tokens
            SET is_revoked = TRUE
            WHERE id = (
                SELECT id
                FROM refresh_tokens
                WHERE token_hash = @TokenHash
                  AND NOT is_revoked
                  AND expires_at_utc > NOW()
                LIMIT 1
                FOR UPDATE
            )
            RETURNING id, user_id AS UserId, token, token_hash AS TokenHash,
                      expires_at_utc AS ExpiresAtUtc, created_at_utc AS CreatedAtUtc, is_revoked AS IsRevoked
            """;

        var consumed = await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            consumeByHashQuery,
            new
            {
                TokenHash = tokenHash,
            },
            transaction);

        if (consumed is not null)
        {
            return consumed;
        }

        const string consumeLegacyQuery = """
            UPDATE refresh_tokens
            SET is_revoked = TRUE
            WHERE id = (
                SELECT id
                FROM refresh_tokens
                WHERE token = @Token
                  AND NOT is_revoked
                  AND expires_at_utc > NOW()
                LIMIT 1
                FOR UPDATE
            )
            RETURNING id, user_id AS UserId, token, token_hash AS TokenHash,
                      expires_at_utc AS ExpiresAtUtc, created_at_utc AS CreatedAtUtc, is_revoked AS IsRevoked
            """;

        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            consumeLegacyQuery,
            new
            {
                Token = token,
            },
            transaction);
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

    public async Task<ExpiredTokensCleanupResult> DeleteExpiredTokensAsync(DateTimeOffset now)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            DELETE FROM refresh_tokens
            WHERE expires_at_utc < @Now
            """;

        var deletedTokensCount = await connection.ExecuteAsync(query, new
        {
            Now = now,
        });

        return new ExpiredTokensCleanupResult(deletedTokensCount);
    }
}
