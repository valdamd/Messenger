namespace Identity.Core.Security;

public sealed class JwtAuthOptions
{
    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string Key { get; init; }

    public int DurationInMinutes { get; init; } = 30;

    public int RefreshTokenExpirationDays { get; init; } = 7;
}
