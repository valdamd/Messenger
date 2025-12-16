namespace Pingo.Messeges.Domain;

public class Message
{
    public Guid Id { get; set; }

    public string Text { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
