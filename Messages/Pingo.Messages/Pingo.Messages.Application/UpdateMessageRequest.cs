using System.ComponentModel.DataAnnotations;

namespace Pingo.Messages.Application;

public class UpdateMessageRequest
{
    [Required(ErrorMessage = "Text is required")]
    [MaxLength(1000, ErrorMessage = "Text cannot exceed 1000 characters")]
    public required string Text { get; init; }
}
