using System.IdentityModel.Tokens.Jwt;
using Identity.Core.Security;
using Identity.Core.Services;
using Microsoft.Extensions.Time.Testing;

namespace Identity.ComponentTests.Tests;

public sealed class TokenProviderTimeTests
{
    [Fact]
    public void GetRefreshTokenExpiration_ShouldUseInjectedTimeProvider()
    {
        var fakeNow = new DateTimeOffset(2026, 1, 10, 8, 30, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(fakeNow);
        var options = new JwtAuthOptions
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "12345678901234567890123456789012",
            DurationInMinutes = 30,
            RefreshTokenExpirationDays = 7,
        };

        ITokenProvider provider = new TokenProvider(options, fakeTimeProvider);

        var expiration = provider.GetRefreshTokenExpiration();

        Assert.Equal(fakeNow.AddDays(7), expiration);
    }

    [Fact]
    public void GenerateAccessToken_ShouldUseInjectedTimeProviderForExpiry()
    {
        var fakeNow = new DateTimeOffset(2026, 1, 10, 8, 30, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(fakeNow);
        var options = new JwtAuthOptions
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "12345678901234567890123456789012",
            DurationInMinutes = 30,
            RefreshTokenExpirationDays = 7,
        };

        ITokenProvider provider = new TokenProvider(options, fakeTimeProvider);

        var accessToken = provider.GenerateAccessToken(Guid.NewGuid(), "user@test.com");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

        Assert.NotNull(jwt.ValidTo);
        Assert.Equal(fakeNow.UtcDateTime.AddMinutes(30), jwt.ValidTo, TimeSpan.FromSeconds(1));
    }
}
