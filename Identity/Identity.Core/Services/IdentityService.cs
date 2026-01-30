using Identity.Core.DTOs.Auth;
using Identity.Core.DTOs.Users;
using Identity.Core.Repositories;
using Identity.Core.Security;

namespace Identity.Core.Services;

public sealed class IdentityService(
    IUserRepository userRepository,
    ICredentialsRepository credentialsRepository,
    IRefreshTokenRepository tokenRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IIdentityService
{
    public async Task<Guid?> RegisterAsync(RegisterUserDto request)
    {
        try
        {
            var existing = await credentialsRepository.GetByEmailAsync(request.Email);
            if (existing is not null)
            {
                return null;
            }

            var user = request.ToEntity();
            var (hash, salt) = passwordHasher.HashPassword(request.Password);
            var credential = new PasswordCredentials
            {
                UserId = user.Id,
                Email = request.Email,
                PasswordHash = hash,
                Salt = salt,
                CreatedAtUtc = DateTimeOffset.UtcNow,
            };

            await userRepository.CreateUserAsync(user);
            await credentialsRepository.AddAsync(credential);

            return user.Id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AccessTokensDto?> LoginAsync(LoginUserDto request)
    {
        var credential = await credentialsRepository.GetByEmailAsync(request.Email);

        if (credential is null || !passwordHasher.VerifyPassword(request.Password, credential.PasswordHash, credential.Salt))
        {
            return null;
        }

        var user = await userRepository.GetUserByIdAsync(credential.UserId);
        if (user is null)
        {
            return null;
        }

        return await GenerateTokensAsync(user.Id, credential.Email);
    }

    public async Task<AccessTokensDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        var token = await tokenRepository.GetValidRefreshTokenAsync(refreshTokenDto.RefreshToken);
        if (token is null)
        {
            return null;
        }

        var user = await userRepository.GetUserByIdAsync(token.UserId);
        if (user is null)
        {
            return null;
        }

        var email = await credentialsRepository.GetEmailByUserIdAsync(token.UserId);
        await tokenRepository.RevokeTokenAsync(token.Id);

        return await GenerateTokensAsync(token.UserId, email!);
    }

    public async Task<UserDto?> GetUserAsync(Guid id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var email = await credentialsRepository.GetEmailByUserIdAsync(id);
        if (email is null)
        {
            return null;
        }

        return user.ToDto(email);
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateUserProfileDto request)
    {
        return await userRepository.UpdateUserAsync(userId, request.Name);
    }

    private async Task<AccessTokensDto> GenerateTokensAsync(Guid userId, string email)
    {
        var accessToken = tokenProvider.GenerateAccessToken(userId, email);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var expiresAt = tokenProvider.GetRefreshTokenExpiration();

        var token = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = refreshToken,
            ExpiresAtUtc = expiresAt,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            IsRevoked = false,
        };

        await tokenRepository.CreateRefreshTokenAsync(token);

        return new AccessTokensDto(accessToken, refreshToken);
    }
}
