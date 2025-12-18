using System.ComponentModel.DataAnnotations;

namespace Message.Message.Api;

public sealed class SendMessageRequest
{
    [Required(ErrorMessage = "Content is required.")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 2000 characters.")]
    public required string Content { get; init; }
}
