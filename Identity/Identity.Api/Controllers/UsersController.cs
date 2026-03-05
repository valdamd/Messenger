using FluentValidation;
using Identity.Api.DTOs.Common;
using Identity.Api.DTOs.Users;
using Identity.Api.Extensions;
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
        var currentUserId = User.GetUserId();
        if (currentUserId is null || currentUserId.Value != id)
        {
            return Forbid();
        }

        var result = await identityService.GetUserAsync(id);
        if (result.IsFailure)
        {
            return result.Error.ToProblemResult(StatusCodes.Status404NotFound);
        }

        var links = acceptHeaderDto.IncludeLinks ? CreateLinksForUser(result.Value.Id) : [];

        return Ok(UserDto.FromProfile(result.Value, links));
    }

    [HttpGet("me", Name = nameof(GetCurrentUser))]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(
        [FromHeader] AcceptHeaderDto acceptHeaderDto)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await identityService.GetUserAsync(userId.Value);
        if (result.IsFailure)
        {
            return result.Error.ToProblemResult(StatusCodes.Status404NotFound);
        }

        var links = acceptHeaderDto.IncludeLinks ? CreateLinksForUser(result.Value.Id) : [];

        return Ok(UserDto.FromProfile(result.Value, links));
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

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await identityService.UpdateProfileAsync(userId.Value, new UpdateUserProfileRequest(request.Name));
        if (result.IsFailure)
        {
            return result.Error.ToProblemResult(StatusCodes.Status404NotFound);
        }

        return NoContent();
    }

    private List<LinkDto> CreateLinksForUser(Guid userId) =>
    [
        linkService.Create(nameof(GetCurrentUser), "self", HttpMethods.Get),
        linkService.Create(nameof(GetUserById), "user-by-id", HttpMethods.Get, new
        {
            id = userId,
        }),
        linkService.Create(nameof(UpdateProfile), "update-profile", HttpMethods.Put),
    ];
}
