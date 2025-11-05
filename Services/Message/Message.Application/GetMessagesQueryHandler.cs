using MediatR;
using Message.Message.Domain;

namespace Message.Message.Application;

internal sealed class GetMessagesQueryHandler(IChatMessageRepository messageRepository)
    : IRequestHandler<GetMessagesQuery, IReadOnlyList<MessageResponse>>
{
    public async Task<IReadOnlyList<MessageResponse>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await messageRepository.GetAllAsync(cancellationToken);

        return messages
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                CreatedOnUtc = m.CreatedOnUtc,
            })
            .ToList();
    }
}
