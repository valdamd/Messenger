namespace Messenger.Client.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);

    Task<bool> RefreshAsync();

    Task LogoutAsync();

    Task<string?> GetAccessTokenAsync();

    Task<bool> IsAuthenticatedAsync();
}
