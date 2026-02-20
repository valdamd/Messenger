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
    [ProducesResponseType<RegisterResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponseDto>> Register(
        [FromBody] RegisterUserDto request,
        [FromServices] IValidator<RegisterUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var result = await identityService.RegisterAsync(
            new RegisterUserRequest(request.Email, request.Name, request.Password));

        if (result.IsFailure)
        {
            return Problem(
                detail: result.Error.Detail,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        return Ok(new RegisterResponseDto(result.Value));
    }

    [HttpPost("login")]
    [ProducesResponseType<AccessTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccessTokensDto>> Login(
        [FromBody] LoginUserDto request,
        [FromServices] IValidator<LoginUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: HttpContext.RequestAborted);

        var result = await identityService.LoginAsync(new LoginUserRequest(request.Email, request.Password));

        if (result.IsFailure)
        {
            var statusCode = result.Error == IdentityErrors.UserNotFound
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status401Unauthorized;

            return Problem(
                detail: result.Error.Detail,
                statusCode: statusCode,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        return Ok(new AccessTokensDto(result.Value.AccessToken, result.Value.RefreshToken));
    }

    [HttpPost("refresh-tokens")]
    [ProducesResponseType<AccessTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokensDto>> RefreshTokens(
        [FromBody] RefreshTokenDto refreshTokenDto,
        [FromServices] IValidator<RefreshTokenDto> validator)
    {
        await validator.ValidateAndThrowAsync(refreshTokenDto, cancellationToken: HttpContext.RequestAborted);

        var result = await identityService.RefreshTokenAsync(new RefreshTokenRequest(refreshTokenDto.RefreshToken));

        if (result.IsFailure)
        {
            return Problem(
                detail: result.Error.Detail,
                statusCode: StatusCodes.Status401Unauthorized,
                extensions: new Dictionary<string, object?>
                {
                    {
                        "code", result.Error.Code
                    },
                });
        }

        return Ok(new AccessTokensDto(result.Value.AccessToken, result.Value.RefreshToken));
    }
}
