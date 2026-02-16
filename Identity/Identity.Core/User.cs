namespace Identity.Core;

public sealed class User
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
