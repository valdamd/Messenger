using Identity.Core.DTOs.Auth;

namespace Identity.Core.DTOs.Users;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserDto dto)
    {
        return new User
        {
            Id = Guid.CreateVersion7(), Name = dto.Name, CreatedAtUtc = DateTimeOffset.UtcNow,
        };
    }

    public static UserDto ToDto(this User user, string email)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = email,
            Name = user.Name,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
        };
    }
}
