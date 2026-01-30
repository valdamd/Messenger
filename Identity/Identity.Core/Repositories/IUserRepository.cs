namespace Identity.Core.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user);

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<bool> UpdateUserAsync(Guid userId, string name);
}
