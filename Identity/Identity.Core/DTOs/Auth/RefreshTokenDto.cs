namespace Identity.Core.DTOs.Auth;

public record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}
