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

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(
    IIdentityService identityService,
    LinkService linkService) : ControllerBase
{
    [HttpGet("{id:guid}", Name = "GetUserById")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(
        Guid id,
        [FromHeader(Name = "Accept")] string? accept)
    {
        if (!IsCurrentUser(id))
        {
            return Forbid();
        }

        var profile = await identityService.GetUserAsync(id);
        if (profile is null)
        {
            return NotFound();
        }

        var acceptHeader = new AcceptHeaderDto
        {
            Accept = accept,
        };
        var links = acceptHeader.IncludeLinks ? linkService.GenerateUserLinks(id) : [];

        var userDto = new UserDto
        {
            Id = profile.Id,
            Email = profile.Email,
            Name = profile.Name,
            Links = links,
        };

        return Ok(userDto);
    }

    [HttpPut("{id:guid}", Name = "UpdateUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        Guid id,
        [FromBody] UpdateUserProfileDto request,
        [FromServices] IValidator<UpdateUserProfileDto> validator)
    {
        if (!IsCurrentUser(id))
        {
            return Forbid();
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var success = await identityService.UpdateProfileAsync(id, new UpdateUserProfileRequest(request.Name));
        return success ? NoContent() : NotFound();
    }

    private bool IsCurrentUser(Guid requestedUserId)
    {
        var subject = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(subject, out var currentUserId) && currentUserId == requestedUserId;
    }
}
