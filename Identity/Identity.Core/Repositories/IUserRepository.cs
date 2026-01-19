using Pingo.Identity;

namespace Identity.Core.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);
}
