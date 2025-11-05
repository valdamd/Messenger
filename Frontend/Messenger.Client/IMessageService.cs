namespace Messenger.Client;

public interface IMessageService
{
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync();

    Task SendMessageAsync(string content);
}
