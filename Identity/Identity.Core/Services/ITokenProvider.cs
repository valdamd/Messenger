namespace Identity.Core.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(Guid userId, string email);

    string GenerateRefreshToken();

    DateTimeOffset GetRefreshTokenExpiration();
}
