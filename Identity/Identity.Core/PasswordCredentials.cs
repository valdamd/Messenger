namespace Identity.Core;

public sealed class PasswordCredentials
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;

    public string Salt { get; init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; init; }
}
