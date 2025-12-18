using Pingo.Messages.Domain;

namespace Pingo.Messages.Application;

public interface IMessageRepository
{
    Task<Message?> GetAsync(Guid id, CancellationToken ct = default);

    Task SaveAsync(Message message, CancellationToken ct = default);
}
