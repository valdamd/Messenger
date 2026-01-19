namespace Identity.Core.Repositories;

public interface ICredentialsRepository
{
    Task AddAsync(PasswordCredentials credentials);

    Task<PasswordCredentials?> GetByEmailAsync(string email);
}
