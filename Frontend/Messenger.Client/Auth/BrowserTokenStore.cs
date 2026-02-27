using System.Text.Json;
using Microsoft.JSInterop;

namespace Messenger.Client.Auth;

public sealed class BrowserTokenStore(IJSRuntime jsRuntime) : ITokenStore
{
    private const string StorageKey = "messenger.auth.tokens";

    public async Task<AuthTokens?> GetAsync()
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AuthTokens>(json);
    }

    public async Task SaveAsync(AuthTokens tokens)
    {
        var json = JsonSerializer.Serialize(tokens);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public Task ClearAsync()
    {
        return jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey).AsTask();
    }
}
