namespace Pingo.Messages.Application;

public interface IMessageService
{
    Task CreateOrUpdateAsync(Guid messageId, string content, CancellationToken cancellationToken = default);

    Task<MessageResponse?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MessageResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
