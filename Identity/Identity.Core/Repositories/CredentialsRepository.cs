using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

internal sealed class CredentialsRepository(NpgsqlDataSource dataSource) : ICredentialsRepository
{
    public async Task AddAsync(PasswordCredentials credentials)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        await AddAsync(credentials, connection, transaction);
        await transaction.CommitAsync();
    }

    public async Task AddAsync(PasswordCredentials credentials, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        const string query = """
            INSERT INTO user_credentials (user_id, email, password_hash, salt, created_at_utc)
            VALUES (@UserId, @Email, @PasswordHash, @Salt, @CreatedAtUtc)
            """;

        await connection.ExecuteAsync(query, credentials, transaction);
    }

    public async Task<PasswordCredentials?> GetByEmailAsync(string email)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            SELECT user_id AS UserId, email, password_hash AS PasswordHash, salt, created_at_utc AS CreatedAtUtc
            FROM user_credentials
            WHERE LOWER(email) = LOWER(@Email)
            """;

        return await connection.QuerySingleOrDefaultAsync<PasswordCredentials>(query, new
        {
            Email = email,
        });
    }

    public async Task<string?> GetEmailByUserIdAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = "SELECT email FROM user_credentials WHERE user_id = @UserId";

        return await connection.ExecuteScalarAsync<string>(query, new
        {
            UserId = userId,
        });
    }
}
