namespace Messenger.Client;

public interface IMessageService
{
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync();

    Task SendOrUpdateAsync(Guid id, string content);

    Task<Guid> SendMessageAsync(string content);
}
