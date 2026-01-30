namespace Identity.Core.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user, PasswordCredentials credential);

    Task<PasswordCredentials?> GetCredentialByEmailAsync(string email);

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<string?> GetEmailByUserIdAsync(Guid userId);

    Task<bool> UpdateUserAsync(Guid userId, string name);
}
