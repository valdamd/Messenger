using Identity.Core.DTOs.Auth;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        var userId = await identityService.RegisterAsync(request);
        return userId == null
            ? BadRequest()
            : Ok(new
            {
                UserId = userId,
            });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        var result = await identityService.LoginAsync(request);
        return result == null ? Unauthorized() : Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await identityService.RefreshTokenAsync(refreshToken);
        return result == null ? Unauthorized() : Ok(result);
    }
}
