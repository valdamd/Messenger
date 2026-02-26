using Dapper;
using Npgsql;

namespace Identity.Core.Repositories;

internal sealed class UserRepository(NpgsqlDataSource dataSource) : IUserRepository
{
    public async Task<Guid> CreateUserAsync(User user)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var userId = await CreateUserAsync(user, connection, transaction);
        await transaction.CommitAsync();
        return userId;
    }

    public async Task<Guid> CreateUserAsync(User user, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        const string query = """
            INSERT INTO users (id, name, created_at_utc)
            VALUES (@Id, @Name, @CreatedAtUtc)
            """;

        await connection.ExecuteAsync(query, user, transaction);
        return user.Id;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            SELECT id, name, created_at_utc AS CreatedAtUtc, updated_at_utc AS UpdatedAtUtc
            FROM users
            WHERE id = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(query, new
        {
            UserId = userId,
        });
    }

    public async Task<bool> UpdateUserAsync(Guid userId, string name, DateTimeOffset updatedAtUtc)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        const string query = """
            UPDATE users
            SET name = @Name, updated_at_utc = @UpdatedAtUtc
            WHERE id = @UserId
            """;

        var affected = await connection.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = name,
            UpdatedAtUtc = updatedAtUtc,
        });

        return affected > 0;
    }
}
