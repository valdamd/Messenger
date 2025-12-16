namespace Message.Message.Application;

public sealed class MessageResponse
{
    public Guid Id { get; init; }

    public string? Content { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
