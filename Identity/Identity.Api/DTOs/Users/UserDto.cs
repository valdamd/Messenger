using Identity.Api.DTOs.Common;
using Identity.Core.Services.Models;

namespace Identity.Api.DTOs.Users;

public sealed class UserDto
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public IReadOnlyList<LinkDto> Links { get; init; } = [];

    public static UserDto FromProfile(UserProfile profile, IReadOnlyList<LinkDto>? links = null) => new()
    {
        Id = profile.Id,
        Email = profile.Email,
        Name = profile.Name,
        Links = links ?? [],
    };
}
