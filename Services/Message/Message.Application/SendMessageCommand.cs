using MediatR;

namespace Message.Message.Application;

public abstract sealed record SendMessageCommand(string Content) : IRequest<Guid>;
