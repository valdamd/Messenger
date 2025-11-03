namespace Message.Message.Domain;

public interface IChatMessageRepository
{
    Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(ChatMessage message);
}
