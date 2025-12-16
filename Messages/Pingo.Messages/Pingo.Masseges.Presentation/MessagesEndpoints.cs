using Pingo.Messages.Application;

namespace Pingo.Messages.Presentation;

public static class MessagesEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/messages/{id:guid}", async (Guid id, string text, MessageService service) =>
        {
            await service.CreateOrUpdateMessageAsync(id, text);
            return Results.NoContent();
        });

        app.MapGet("/api/messages/{id:guid}", async (Guid id, IMessageRepository repository) =>
        {
            var message = await repository.GetAsync(id);
            return message is not null ? Results.Ok(message) : Results.NotFound();
        });
    }
}
