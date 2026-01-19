namespace Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task DeleteAsync(Guid id);
}
