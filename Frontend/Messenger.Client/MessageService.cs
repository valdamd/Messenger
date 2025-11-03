using System.Net.Http.Json;

namespace Messenger.Client;

public sealed class MessageService(HttpClient httpClient) : IMessageService
{
    public async Task<List<MessageDto>> GetMessagesAsync()
    {
        var messages = await httpClient.GetFromJsonAsync<List<MessageDto>>("api/messages");
        return messages ?? new List<MessageDto>();
    }

    public async Task SendMessageAsync(string content)
    {
        var request = new { Content = content };
        var response = await httpClient.PostAsJsonAsync("api/messages", request);
        response.EnsureSuccessStatusCode();
    }
}
