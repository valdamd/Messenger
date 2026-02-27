using Messenger.Client;
using Messenger.Client.Auth;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped<ITokenStore, BrowserTokenStore>();
builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddScoped<RetryHttpMessageHandler>();

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5100/");
});

builder.Services
    .AddHttpClient<IMessageService, MessageService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5132/");
    })
    .AddHttpMessageHandler<RetryHttpMessageHandler>()
    .AddHttpMessageHandler<AuthMessageHandler>();

await builder.Build().RunAsync();
