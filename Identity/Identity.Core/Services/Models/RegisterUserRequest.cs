namespace Identity.Core.Services.Models;

public sealed record RegisterUserRequest(string Email, string Name, string Password);
