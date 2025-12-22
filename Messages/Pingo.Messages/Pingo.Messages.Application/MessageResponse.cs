namespace Pingo.Messages.Application;

public sealed record MessageResponse(
    Guid Id,
    string Content,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
