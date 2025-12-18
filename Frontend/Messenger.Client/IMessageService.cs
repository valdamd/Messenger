using Message.Message.Application;

namespace Messenger.Client;

public interface IMessageService
{
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync();

    Task SendMessageAsync(string content);

    Task SendOrUpdateAsync(Guid id, string content);

    Task<IReadOnlyList<MessageResponse>> GetAllAsync();
}
