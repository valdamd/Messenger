namespace Pingo.Messages.Domain;

public sealed class ChatMessage
{
    private ChatMessage()
    {
    }

    public Guid Id { get; private init; }

    public string Content { get; private init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public DateTimeOffset? UpdatedAtUtc { get; private init; }

    public static ChatMessage Create(Guid id, string content)
    {
        return new ChatMessage
        {
            Id = id,
            Content = content,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = null,
        };
    }

    public ChatMessage UpdateContent(string content)
    {
        if (Content == content)
        {
            return this;
        }

        return new ChatMessage
        {
            Id = Id, Content = content, CreatedAtUtc = CreatedAtUtc, UpdatedAtUtc = DateTimeOffset.UtcNow,
        };
    }
}
