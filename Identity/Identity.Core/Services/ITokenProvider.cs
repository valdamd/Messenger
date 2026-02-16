namespace Identity.Core.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(Guid userId, string email);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTimeOffset GetRefreshTokenExpiration();
}
