using Identity.Core.DTOs.Auth;
using Identity.Core.DTOs.Users;
using Pingo.Identity;

namespace Identity.Core.Services;

public interface IIdentityService
{
    Task<Guid?> RegisterAsync(RegisterUserDto request);

    Task<AccessTokensDto?> LoginAsync(LoginUserDto request);

    Task<AccessTokensDto?> RefreshTokenAsync(string refreshToken);

    Task<UserDto?> GetUserAsync(Guid id);

    Task<bool> UpdateProfileAsync(Guid userId, UpdateUserProfileDto request);
}
