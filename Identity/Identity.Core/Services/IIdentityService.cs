using Identity.Core.Services.Models;

namespace Identity.Core.Services;

public interface IIdentityService
{
    Task<Guid?> RegisterAsync(RegisterUserRequest request);

    Task<AccessTokens?> LoginAsync(LoginUserRequest request);

    Task<AccessTokens?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);

    Task<UserProfile?> GetUserAsync(Guid id);

    Task<bool> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);
}
