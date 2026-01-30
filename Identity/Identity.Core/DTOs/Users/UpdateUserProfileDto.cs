namespace Identity.Core.DTOs.Users;

public sealed record UpdateUserProfileDto
{
    public required string Name { get; init; }
}
