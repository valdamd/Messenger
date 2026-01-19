using Identity.Core.DTOs.Users;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IdentityService identityService) : ControllerBase
{
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileDto request)
    {
        var success = await identityService.UpdateProfileAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var user = await identityService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user.ToDto());
    }
}
