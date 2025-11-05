using MediatR;

namespace Message.Message.Application;

public sealed record SendMessageCommand(string Content) : IRequest<Guid>;
