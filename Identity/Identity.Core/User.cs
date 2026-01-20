namespace Identity.Core;

public sealed class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }
}
