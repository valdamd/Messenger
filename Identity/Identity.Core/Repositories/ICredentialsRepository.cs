using Npgsql;

namespace Identity.Core.Repositories;

public interface ICredentialsRepository
{
    Task AddAsync(PasswordCredentials credentials);

    Task AddAsync(PasswordCredentials credentials, NpgsqlConnection connection, NpgsqlTransaction transaction);

    Task<PasswordCredentials?> GetByEmailAsync(string email);

    Task<string?> GetEmailByUserIdAsync(Guid userId);
}
