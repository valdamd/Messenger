using Identity.Core.DTOs.Auth;
using Identity.Core.DTOs.Common;
using Pingo.Identity;

namespace Identity.Core.DTOs.Users;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserDto dto)
    {
        return new User
        {
            Id = Guid.CreateVersion7(), Name = dto.Name, Email = dto.Email.ToLowerInvariant().Trim(), CreatedAtUtc = DateTimeOffset.UtcNow,
        };
    }

    public static UserDto ToDto(this User user, List<LinkDto>? links = null)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            Links = links ?? [],
        };
    }
}
