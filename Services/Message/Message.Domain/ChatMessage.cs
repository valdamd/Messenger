namespace Message.Message.Domain;

public class ChatMessage(Guid id, string content, DateTime createdOnUtc)
{
    public Guid Id { get; private set; } = id;

    public string Content { get; private set; } = content;

    public DateTime CreatedOnUtc { get; private set; } = createdOnUtc;

    public static ChatMessage Create(string content, DateTime createdOnUtc)
    {
        return new ChatMessage(Guid.NewGuid(), content, createdOnUtc);
    }
}
