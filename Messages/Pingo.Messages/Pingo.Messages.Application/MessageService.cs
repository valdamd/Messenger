using Pingo.Message.Domain;
using Pingo.Messages.Domain;
using Pingo.Messages.Domain.Messages;

namespace Pingo.Messages.Application;

public sealed class MessageService(IChatMessageRepository messageRepository, IUnitOfWork unitOfWork) : IMessageService
{
    public async Task CreateOrUpdateAsync(Guid messageId, string content, CancellationToken cancellationToken = default)
    {
        var existingMessage = await messageRepository.GetAsync(messageId, cancellationToken);

        if (existingMessage is null)
        {
            var newMessage = ChatMessage.Create(messageId, content);
            messageRepository.Insert(newMessage);
        }
        else
        {
            existingMessage.UpdateContent(content);
            messageRepository.Update(existingMessage);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<MessageResponse?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await messageRepository.GetAsync(messageId, cancellationToken);

        return message is null ? null : MapToResponse(message);
    }

    public async Task<IReadOnlyList<MessageResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await messageRepository.GetAllAsync(cancellationToken);

        return messages.Select(MapToResponse).ToList();
    }

    private static MessageResponse MapToResponse(ChatMessage message)
    {
        return new MessageResponse(
            message.Id,
            message.Content,
            message.CreatedAtUtc,
            message.UpdatedAtUtc);
    }
}
