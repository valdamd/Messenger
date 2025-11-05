using System.Net.Http.Json;

namespace Messenger.Client;

public sealed class MessageService(HttpClient httpClient) : IMessageService
{
    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync()
    {
        var messages = await httpClient.GetFromJsonAsync<List<MessageDto>?>("api/messages");
        if (messages is null || messages.Count == 0)
        {
            return Array.Empty<MessageDto>();
        }

        return messages.AsReadOnly();
    }

    public async Task SendMessageAsync(string content)
    {
        var request = new { Content = content };
        var response = await httpClient.PostAsJsonAsync("api/messages", request);
        response.EnsureSuccessStatusCode();
    }
}
