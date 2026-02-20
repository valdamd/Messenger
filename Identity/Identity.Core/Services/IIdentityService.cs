using CSharpFunctionalExtensions;
using Identity.Core.Services.Models;

namespace Identity.Core.Services;

public interface IIdentityService
{
    Task<Result<Guid, IdentityError>> RegisterAsync(RegisterUserRequest request);

    Task<Result<AccessTokens, IdentityError>> LoginAsync(LoginUserRequest request);

    Task<Result<AccessTokens, IdentityError>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);

    Task<Result<UserProfile, IdentityError>> GetUserAsync(Guid id);

    Task<UnitResult<IdentityError>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);
}
