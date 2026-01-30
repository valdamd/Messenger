using FluentValidation;
using Identity.Api.Services;
using Identity.Core.DTOs.Common;
using Identity.Core.DTOs.Users;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(
    IIdentityService identityService,
    LinkService linkService) : ControllerBase
{
    [HttpGet("{id:guid}", Name = "GetUserById")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(
        Guid id,
        [FromHeader(Name = "Accept")] string? accept)
    {
        var userDto = await identityService.GetUserAsync(id);
        if (userDto is null)
        {
            return NotFound();
        }

        var acceptHeader = new AcceptHeaderDto
        {
            Accept = accept,
        };
        if (acceptHeader.IncludeLinks)
        {
            userDto.Links = linkService.GenerateUserLinks(id);
        }

        return Ok(userDto);
    }

    [HttpPut("{id:guid}", Name = "UpdateUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        Guid id,
        [FromBody] UpdateUserProfileDto request,
        [FromServices] IValidator<UpdateUserProfileDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var success = await identityService.UpdateProfileAsync(id, request);
        return success ? NoContent() : NotFound();
    }
}
