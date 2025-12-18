namespace Message.Message.Domain;

public sealed class ChatMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static ChatMessage Create(string content, DateTimeOffset createdAt)
    {
        return new ChatMessage
        {
            Id = Guid.NewGuid(),
            Content = content,
            CreatedAt = createdAt,
        };
    }

    public void Update(string content, DateTimeOffset updatedAt)
    {
        Content = content;
        UpdatedAt = updatedAt;
    }
}
