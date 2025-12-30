using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Pingo.Messages.Application;
using Pingo.Messages.ComponentTests.Abstractions;

namespace Pingo.Messages.ComponentTests;

public sealed class MessagesEndpointsTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetMessages_WhenNoMessages_ReturnsEmptyList()
    {
        var response = await HttpClient.GetAsync("/api/messages");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponse>>();
        messages.Should().NotBeNull();
        messages.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateMessage_WhenValidRequest_MessageAppearsInList()
    {
        var messageId = Guid.NewGuid();
        const string content = "Hello, World!";
        var request = new
        {
            Content = content,
        };

        var putResponse = await HttpClient.PutAsJsonAsync($"/api/messages/{messageId}", request);

        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await HttpClient.GetAsync("/api/messages");
        var messages = await getResponse.Content.ReadFromJsonAsync<List<MessageResponse>>();

        messages.Should().NotBeNull();
        messages.Should().HaveCount(1);
        messages![0].Id.Should().Be(messageId);
        messages[0].Content.Should().Be(content);
        messages[0].CreatedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        messages[0].UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task CreateTwoMessages_ReturnsMessagesInOrderOfCreation()
    {
        var firstMessageId = Guid.NewGuid();
        var secondMessageId = Guid.NewGuid();

        await HttpClient.PutAsJsonAsync($"/api/messages/{firstMessageId}", new
        {
            Content = "First message",
        });

        await Task.Delay(50);

        await HttpClient.PutAsJsonAsync($"/api/messages/{secondMessageId}", new
            {
                Content = "Second message",
            });

        var response = await HttpClient.GetAsync("/api/messages");
        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponse>>();

        messages.Should().NotBeNull();
        messages.Should().HaveCount(2);

        messages![0].Id.Should().Be(firstMessageId);
        messages[0].Content.Should().Be("First message");

        messages[1].Id.Should().Be(secondMessageId);
        messages[1].Content.Should().Be("Second message");

        messages[0].CreatedAtUtc.Should().BeBefore(messages[1].CreatedAtUtc);
    }

    [Fact]
    public async Task UpdateMessage_WhenSameIdDifferentContent_UpdatesMessageAndSetsUpdatedAt()
    {
        var messageId = Guid.NewGuid();
        const string originalContent = "Original content";
        const string updatedContent = "Updated content";

        await HttpClient.PutAsJsonAsync($"/api/messages/{messageId}", new
            {
                Content = originalContent,
            });

        await Task.Delay(100);

        var updateResponse = await HttpClient.PutAsJsonAsync($"/api/messages/{messageId}", new
            {
                Content = updatedContent,
            });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await HttpClient.GetAsync("/api/messages");
        var messages = await getResponse.Content.ReadFromJsonAsync<List<MessageResponse>>();

        messages.Should().NotBeNull();
        messages.Should().HaveCount(1);

        var message = messages![0];
        message.Id.Should().Be(messageId);
        message.Content.Should().Be(updatedContent);

        message.UpdatedAtUtc.Should().NotBeNull();
        message.UpdatedAtUtc.Should().BeAfter(message.CreatedAtUtc);
    }

    [Fact]
    public async Task GetMessageById_WhenMessageExists_ReturnsMessage()
    {
        var messageId = Guid.NewGuid();
        const string content = "Test message";

        await HttpClient.PutAsJsonAsync($"/api/messages/{messageId}", new
            {
                Content = content,
            });

        var response = await HttpClient.GetAsync($"/api/messages/{messageId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadFromJsonAsync<MessageResponse>();
        message.Should().NotBeNull();
        message!.Id.Should().Be(messageId);
        message.Content.Should().Be(content);
    }

    [Fact]
    public async Task GetMessageById_WhenMessageDoesNotExist_ReturnsNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var response = await HttpClient.GetAsync($"/api/messages/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
