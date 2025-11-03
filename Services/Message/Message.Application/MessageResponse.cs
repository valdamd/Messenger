namespace Message.Message.Application;

public class MessageResponse
{
    public Guid Id { get; init; }

    public string? Content { get; init; }

    public DateTime CreatedOnUtc { get; init; }
}
