using System.Net.Http.Json;
using Messenger.Client.Auth;

namespace Messenger.Client;

public sealed class MessageService(HttpClient httpClient) : IMessageService
{
    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync()
    {
        var messages = await httpClient.GetFromJsonAsync<List<MessageDto>>("api/messages");

        if (messages is null || messages.Count == 0)
        {
            return Array.Empty<MessageDto>();
        }

        return messages.AsReadOnly();
    }

    public async Task<Guid> SendMessageAsync(string content)
    {
        var messageId = Guid.NewGuid();
        await SendOrUpdateAsync(messageId, content);
        return messageId;
    }

    public async Task SendOrUpdateAsync(Guid id, string content)
    {
        var request = new
        {
            Content = content,
        };
        var response = await httpClient.PutAsJsonAsync($"api/messages/{id}", request);
        response.EnsureSuccessStatusCode();
    }
}
