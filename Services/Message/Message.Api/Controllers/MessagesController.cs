using Message.Message.Api;
using Microsoft.AspNetCore.Mvc;
using Pingo.Messages.Application;

namespace Message.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpPut("{messageId:guid}")]
    public async Task<IActionResult> CreateOrUpdate(
        Guid messageId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        await messageService.CreateOrUpdateAsync(messageId, request.Content, cancellationToken);

        return NoContent();
    }

    [HttpGet("{messageId:guid}")]
    public async Task<IActionResult> GetById(Guid messageId, CancellationToken cancellationToken)
    {
        var message = await messageService.GetByIdAsync(messageId, cancellationToken);

        if (message is null)
        {
            return NotFound();
        }

        return Ok(message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var messages = await messageService.GetAllAsync(cancellationToken);

        return Ok(messages);
    }
}
