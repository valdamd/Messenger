using FluentValidation;
using Identity.Core.DTOs.Users;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(
    IIdentityService identityService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var userDto = await identityService.GetUserAsync(id);
        return userDto is null ? NotFound() : Ok(userDto);
    }

    [HttpPut("{id:guid}")]
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
