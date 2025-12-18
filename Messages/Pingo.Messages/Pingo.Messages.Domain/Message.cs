namespace Pingo.Messages.Domain;

public sealed class Message
{
    public Guid Id { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Message Create(Guid id, string text)
    {
        return new Message
        {
            Id = id,
            Text = text,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void UpdateText(string text)
    {
        Text = text;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
