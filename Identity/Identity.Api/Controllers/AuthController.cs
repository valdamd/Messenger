using FluentValidation;
using Identity.Api.DTOs.Auth;
using Identity.Core.Services;
using Identity.Core.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserDto request,
        [FromServices] IValidator<RegisterUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var userId = await identityService.RegisterAsync(
            new RegisterUserRequest(request.Email, request.Name, request.Password));

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
    public async Task<IActionResult> Login(
        [FromBody] LoginUserDto request,
        [FromServices] IValidator<LoginUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var result = await identityService.LoginAsync(new LoginUserRequest(request.Email, request.Password));
        return result is null
            ? Unauthorized()
            : Ok(new AccessTokensDto(result.AccessToken, result.RefreshToken));
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AccessTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto refreshTokenDto,
        [FromServices] IValidator<RefreshTokenDto> validator)
    {
        await validator.ValidateAndThrowAsync(refreshTokenDto, cancellationToken: HttpContext.RequestAborted);

        var result = await identityService.RefreshTokenAsync(new RefreshTokenRequest(refreshTokenDto.RefreshToken));
        return result is null
            ? Unauthorized()
            : Ok(new AccessTokensDto(result.AccessToken, result.RefreshToken));
    }
}
