using Microsoft.AspNetCore.Mvc;
using Pingo.Messages;

namespace Message.Message.Api;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _service;

    public MessagesController(IMessageService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPut("{messageId:guid}")]
    public async Task<IActionResult> SendOrUpdate([FromRoute] Guid messageId, [FromBody] SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Text))
        {
            return BadRequest("Text is required.");
        }

        await _service.SendOrUpdateAsync(messageId, request.Text);
        return NoContent();
    }
}
