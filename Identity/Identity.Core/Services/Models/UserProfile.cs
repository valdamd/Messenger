namespace Identity.Core.Services.Models;

public sealed record UserProfile(
    Guid Id,
    string Email,
    string Name);
