using Identity.Api.DTOs.Common;

namespace Identity.Api.DTOs.Users;

public sealed class UserDto
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public IReadOnlyList<LinkDto> Links { get; init; } = [];
}
