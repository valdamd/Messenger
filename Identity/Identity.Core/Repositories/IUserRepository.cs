using Npgsql;

namespace Identity.Core.Repositories;

interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user);

    Task<Guid> CreateUserAsync(User user, NpgsqlConnection connection, NpgsqlTransaction transaction);

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<bool> UpdateUserAsync(Guid userId, string name, DateTimeOffset updatedAtUtc);
}
