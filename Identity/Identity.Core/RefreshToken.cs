namespace Identity.Core;

public sealed class RefreshToken
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string Token { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; init; }
}
