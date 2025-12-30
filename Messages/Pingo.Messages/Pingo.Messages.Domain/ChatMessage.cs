namespace Pingo.Messages.Domain;

public sealed class ChatMessage
{
    private ChatMessage()
    {
    }

    public Guid Id { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

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

    public void UpdateContent(string content)
    {
        if (Content == content)
        {
            return;
        }

        Content = content;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
