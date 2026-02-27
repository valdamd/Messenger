namespace Messenger.Client.Auth;

public sealed class AuthTokens
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;
}
