using Identity.Core.DTOs.Auth;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        var userId = await identityService.RegisterAsync(request);

        return userId is null
            ? BadRequest(new
            {
                error = "Email уже зарегистрирован",
            })
            : Ok(new
            {
                userId,
            });
    }

    [HttpPost("login")]
    [ProducesResponseType<AccessTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        var result = await identityService.LoginAsync(request);
        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AccessTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await identityService.RefreshTokenAsync(refreshToken);
        return result is null ? Unauthorized() : Ok(result);
    }
}
