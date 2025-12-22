namespace Pingo.Messages.Application;

public sealed record MessageResponse(
    Guid Id,
    string Content,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
