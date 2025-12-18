using Messenger.Client;
using Microsoft.AspNetCore.Mvc;

namespace Message.Message.Api;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(IMessageService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var messages = await service.GetAllAsync();
        return Ok(messages);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<IActionResult> SendOrUpdate(
        [FromRoute] Guid messageId,
        [FromBody] SendMessageRequest? request)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await service.SendOrUpdateAsync(messageId, request.Content);
        return NoContent();
    }
}
