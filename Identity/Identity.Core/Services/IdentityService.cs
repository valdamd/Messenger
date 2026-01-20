using Identity.Core.DTOs.Auth;
using Identity.Core.DTOs.Users;
using Identity.Core.Repositories;
using Identity.Core.Security;
using Pingo.Identity;

namespace Identity.Core.Services;

public sealed class IdentityService(
    IUserRepository userRepository,
    IRefreshTokenRepository tokenRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IIdentityService
{
    public async Task<Guid?> RegisterAsync(RegisterUserDto request)
    {
        try
        {
            var existing = await userRepository.GetCredentialByEmailAsync(request.Email);
            if (existing is not null)
            {
                return null;
            }

            var user = request.ToEntity();
            var credential = new PasswordCredentials()
            {
                UserId = user.Id,
                Email = user.Email,
                PasswordHash = passwordHasher.HashPassword(request.Password),
                CreatedAtUtc = DateTimeOffset.UtcNow,
            };

            return await userRepository.CreateUserAsync(user, credential);
        }
        catch
        {
            return null;
        }
    }

    public async Task<AccessTokensDto?> LoginAsync(LoginUserDto request)
    {
        var credential = await userRepository.GetCredentialByEmailAsync(request.Email);

        if (credential is null || !passwordHasher.VerifyPassword(credential.PasswordHash, request.Password))
        {
            return null;
        }

        var user = await userRepository.GetUserByIdAsync(credential.UserId);
        if (user is null)
        {
            return null;
        }

        return await GenerateTokensAsync(user.Id, user.Email);
    }

    public async Task<AccessTokensDto?> RefreshTokenAsync(string refreshToken)
    {
        var token = await tokenRepository.GetValidRefreshTokenAsync(refreshToken);
        if (token is null)
        {
            return null;
        }

        var user = await userRepository.GetUserByIdAsync(token.UserId);
        if (user is null)
        {
            return null;
        }

        await tokenRepository.RevokeTokenAsync(token.Id);

        return await GenerateTokensAsync(token.UserId, user.Email);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await userRepository.GetUserByIdAsync(id);
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
            Id = Guid.NewGuid(),
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
