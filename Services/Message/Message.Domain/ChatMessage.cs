namespace Message.Message.Domain;

public sealed class ChatMessage(Guid id, string content, DateTimeOffset createdAt)
{
    public Guid Id { get; private set; } = id;

    public string Content { get; private set; } = content;

    public DateTimeOffset CreatedAt { get; private set; } = createdAt;

    public static ChatMessage Create(string content, DateTimeOffset createdAt)
    {
        return new ChatMessage(Guid.NewGuid(), content, createdAt);
    }
}
