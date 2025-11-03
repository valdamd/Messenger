using MediatR;
using Message.Message.Application;
using Microsoft.AspNetCore.Mvc;

namespace Message.Message.Api;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMessages(CancellationToken cancellationToken)
    {
        var query = new GetMessagesQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendMessageCommand(request.Content);

        var result = await sender.Send(command, cancellationToken);

        return Ok(result);
    }
}
