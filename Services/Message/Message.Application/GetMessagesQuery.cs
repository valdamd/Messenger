using MediatR;

namespace Message.Message.Application;

public sealed record GetMessagesQuery : IRequest<IReadOnlyList<MessageResponse>>
{
    public static readonly string Marker = nameof(GetMessagesQuery);
}
