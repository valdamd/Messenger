using System.ComponentModel.DataAnnotations;

namespace Message.Message.Api;

public sealed class SendMessageRequest
{
    public required string Content { get; init; }
}
