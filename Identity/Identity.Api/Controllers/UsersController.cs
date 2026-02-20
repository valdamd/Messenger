using System.Security.Claims;
using FluentValidation;
using Identity.Api.DTOs.Common;
using Identity.Api.DTOs.Users;
using Identity.Api.Services;
using Identity.Core.Services;
using Identity.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public sealed class UsersController(
    IIdentityService identityService,
    LinkService linkService) : ControllerBase
{
    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(
        Guid id,
        [FromHeader] AcceptHeaderDto acceptHeaderDto)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null || currentUserId.Value != id)
        {
            return Forbid();
        }

        var result = await identityService.GetUserAsync(id);
        if (result.IsFailure)
        {
            return Problem(
                detail: result.Error.Detail,
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        var links = acceptHeaderDto.IncludeLinks ? CreateLinksForUser(result.Value.Id) : [];

        return Ok(MapToDto(result.Value, links));
    }

    [HttpGet("me", Name = nameof(GetCurrentUser))]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(
        [FromHeader] AcceptHeaderDto acceptHeaderDto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await identityService.GetUserAsync(userId.Value);
        if (result.IsFailure)
        {
            return Problem(
                detail: result.Error.Detail,
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        var links = acceptHeaderDto.IncludeLinks ? CreateLinksForUser(result.Value.Id) : [];

        return Ok(MapToDto(result.Value, links));
    }

    [HttpPut("me/profile", Name = nameof(UpdateProfile))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileDto request,
        [FromServices] IValidator<UpdateUserProfileDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await identityService.UpdateProfileAsync(userId.Value, new UpdateUserProfileRequest(request.Name));
        if (result.IsFailure)
        {
            return Problem(
                detail: result.Error.Detail,
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        return NoContent();
    }

    private static UserDto MapToDto(UserProfile profile, IReadOnlyList<LinkDto>? links = null) => new()
    {
        Id = profile.Id,
        Email = profile.Email,
        Name = profile.Name,
        Links = links ?? [],
    };

    private List<LinkDto> CreateLinksForUser(Guid userId) =>
    [
        linkService.Create(nameof(GetCurrentUser), "self", HttpMethods.Get),
        linkService.Create(nameof(GetUserById), "user-by-id", HttpMethods.Get, new
        {
            id = userId,
        }),
        linkService.Create(nameof(UpdateProfile), "update-profile", HttpMethods.Put),
    ];

    private Guid? GetCurrentUserId()
    {
        var subject = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(subject, out var userId) ? userId : null;
    }
}
