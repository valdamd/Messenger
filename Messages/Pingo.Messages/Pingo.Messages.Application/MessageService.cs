using Pingo.Messeges.Domain;

namespace Pingo.Messages.Application;

internal sealed class MessageService(IMessageRepository repository)
{
    public async Task CreateOrUpdateMessageAsync(Guid id, string text)
    {
        var message = await repository.GetAsync(id) ?? new Message
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        message.Text = text;
        message.UpdatedAt = DateTimeOffset.UtcNow;

        await repository.SaveAsync(message);
    }
}
