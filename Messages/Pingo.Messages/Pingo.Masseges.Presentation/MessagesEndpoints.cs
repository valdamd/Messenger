// MessagesEndpoint.cs
using Messages.Application;
using Messages.Common.Domain;
using Messages.Common.Presentation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Messages.Presentation.Endpoints;

public sealed class MessagesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/messages/{id:guid}", async (
                Guid id,
                UpdateMessageCommand command,
                MessageService service,
                CancellationToken ct) =>
            {
                var result = await service.CreateOrUpdateAsync(id, command.Text, ct);
                return result.IsSuccess ? Results.NoContent() : ApiResults.Problem(result);
            })
            .WithName("CreateOrUpdateMessage")
            .WithOpenApi();

        app.MapGet("/api/messages/{id:guid}", async (
                Guid id,
                MessageService service,
                CancellationToken ct) =>
            {
                var result = await service.GetAsync(id, ct);
                return result.Match(
                    success: dto => Results.Ok(dto),
                    failure: ApiResults.Problem);
            })
            .WithName("GetMessage")
            .WithOpenApi();
    }
}

// UpdateMessageCommand.cs (вместо Request — как в DDD)
public sealed record UpdateMessageCommand(string Text);
