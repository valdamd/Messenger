using MediatR;

namespace Message.Message.Application;

public sealed record GetMessagesQuery : IRequest<IReadOnlyList<MessageResponse>>;
