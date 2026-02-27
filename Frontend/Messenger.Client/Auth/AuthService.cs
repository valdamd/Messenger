using System.Net.Http.Json;

namespace Messenger.Client.Auth;

public sealed class AuthService(HttpClient identityHttpClient, ITokenStore tokenStore) : IAuthService
{
    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await identityHttpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password,
        });

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var tokens = await response.Content.ReadFromJsonAsync<AuthTokens>();
        if (tokens is null || string.IsNullOrWhiteSpace(tokens.AccessToken) || string.IsNullOrWhiteSpace(tokens.RefreshToken))
        {
            return false;
        }

        await tokenStore.SaveAsync(tokens);
        return true;
    }

    public async Task<bool> RefreshAsync()
    {
        var currentTokens = await tokenStore.GetAsync();
        if (currentTokens is null || string.IsNullOrWhiteSpace(currentTokens.RefreshToken))
        {
            return false;
        }

        var response = await identityHttpClient.PostAsJsonAsync("api/auth/refresh-tokens", new RefreshTokenRequest
        {
            RefreshToken = currentTokens.RefreshToken,
        });

        if (!response.IsSuccessStatusCode)
        {
            await tokenStore.ClearAsync();
            return false;
        }

        var newTokens = await response.Content.ReadFromJsonAsync<AuthTokens>();
        if (newTokens is null || string.IsNullOrWhiteSpace(newTokens.AccessToken) || string.IsNullOrWhiteSpace(newTokens.RefreshToken))
        {
            await tokenStore.ClearAsync();
            return false;
        }

        await tokenStore.SaveAsync(newTokens);
        return true;
    }

    public Task LogoutAsync()
    {
        return tokenStore.ClearAsync();
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var tokens = await tokenStore.GetAsync();
        return tokens?.AccessToken;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var tokens = await tokenStore.GetAsync();
        return tokens is not null &&
               !string.IsNullOrWhiteSpace(tokens.AccessToken) &&
               !string.IsNullOrWhiteSpace(tokens.RefreshToken);
    }
}
