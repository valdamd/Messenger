namespace Messenger.Client;

public sealed class MessageDto
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedOnUtc { get; set; }
}
