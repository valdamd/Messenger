using Identity.Core.DTOs.Common;

namespace Identity.Core.DTOs.Users;

public class UserDto : ILinksResponse
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public List<LinkDto> Links { get; set; }
}
