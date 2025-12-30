namespace Pingo.Messages.Domain;

public interface IChatMessageRepository
{
    Task<ChatMessage?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default);

    void Insert(ChatMessage message);

    void Update(ChatMessage message);
}
