using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

public sealed class UserRepository(NpgsqlDataSource dataSource) : IUserRepository
{
    public async Task<Guid> CreateUserAsync(User user, PasswordCredentials credential)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            const string insertUser = """
                INSERT INTO users (id, name, created_at_utc)
                VALUES (@Id, @Name, @CreatedAtUtc)
                """;

            await connection.ExecuteAsync(insertUser, user, transaction);

            const string insertCredential = """
                INSERT INTO user_credentials (user_id, email, password_hash, salt, created_at_utc)
                VALUES (@UserId, @Email, @PasswordHash, @Salt, @CreatedAtUtc)
                """;

            await connection.ExecuteAsync(insertCredential, credential, transaction);

            await transaction.CommitAsync();
            return user.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PasswordCredentials?> GetCredentialByEmailAsync(string email)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            SELECT user_id AS UserId, email, password_hash AS PasswordHash, salt, created_at_utc AS CreatedAtUtc
            FROM user_credentials
            WHERE LOWER(email) = LOWER(@Email)
            """;

        return await connection.QuerySingleOrDefaultAsync<PasswordCredentials>(
            query,
            new
                {
                    Email = email,
                });
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            SELECT id, name, created_at_utc AS CreatedAtUtc, updated_at_utc AS UpdatedAtUtc
            FROM users
            WHERE id = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            query,
            new
            {
                UserId = userId,
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

    public async Task<bool> UpdateUserAsync(Guid userId, string name)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            UPDATE users
            SET name = @Name, updated_at_utc = @UpdatedAtUtc
            WHERE id = @UserId
            """;

        var affected = await connection.ExecuteAsync(
            query,
            new
                {
                    UserId = userId, Name = name, UpdatedAtUtc = DateTimeOffset.UtcNow,
                });

        return affected > 0;
    }
}
