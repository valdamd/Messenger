using Identity.Core.Services.Models;

namespace Identity.Core.Services;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserRequest request, DateTimeOffset createdAtUtc)
    {
        return new User
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public static UserProfile ToProfile(this User user, string email)
    {
        return new UserProfile(
            user.Id,
            email,
            user.Name);
    }
}
