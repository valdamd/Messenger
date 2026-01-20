namespace Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task<Guid> CreateRefreshTokenAsync(RefreshToken token);

    Task<RefreshToken?> GetValidRefreshTokenAsync(string token);

    Task RevokeTokenAsync(Guid tokenId);
}
