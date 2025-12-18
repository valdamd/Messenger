using Pingo.Messages.Domain;

namespace Pingo.Messages.Application;

public sealed class MessageService(IMessageRepository repository)
{
    public async Task<Result> CreateOrUpdateAsync(Guid id, string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Error.Problem("Message.Text.Empty", "Message text cannot be empty or whitespace.");
        }

        var message = await repository.GetAsync(id, ct) ?? Message.Create(id, text);

        message.UpdateText(text);

        await repository.SaveAsync(message, ct);

        return Result.Success();
    }

    public async Task<Result<Message>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var message = await repository.GetAsync(id, ct);

        return message is null
            ? Result<Message>.Failure(Error.NotFound("Message.NotFound", "Message not found"))
            : Result<Message>.Success(message);
    }
}
