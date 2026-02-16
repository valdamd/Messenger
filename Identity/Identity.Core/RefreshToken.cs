namespace Identity.Core;

public sealed class RefreshToken
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string? Token { get; init; }

    public string? TokenHash { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public bool IsRevoked { get; init; }
}
