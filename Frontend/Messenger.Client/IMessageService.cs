namespace Messenger.Client;

public interface IMessageService
{
    Task<List<MessageDto>> GetMessagesAsync();

    Task SendMessageAsync(string content);
}
