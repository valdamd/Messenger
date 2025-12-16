using Pingo.Messeges.Domain;

namespace Pingo.Messages.Application;

public interface IMessageRepository
{
    Task SaveAsync(Message message);

    Task<Message?> GetAsync(Guid messageId);
}
