using MediatR;
using Message.Message.Domain;

namespace Message.Message.Application;

internal sealed class SendMessageCommandHandler(
    IChatMessageRepository messageRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SendMessageCommand, Guid>
{
    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = ChatMessage.Create(request.Content, DateTimeOffset.UtcNow);

        messageRepository.Add(message);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}
