using Hangfire;
using Hangfire.Redis.StackExchange;
using Identity.Api;
using Identity.Api.Jobs;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddBackgroundServices();

var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
var hangfireEnabled = !string.IsNullOrWhiteSpace(redisConnectionString);
var hangfireOperational = false;

if (hangfireEnabled)
{
    var safeConnectionString = EnsureAbortConnectFalse(redisConnectionString!);

    try
    {
        using var probe = ConnectionMultiplexer.Connect(safeConnectionString);
        probe.GetDatabase().Ping();

        builder.Services.AddHangfire(configuration =>
            configuration.UseRedisStorage(safeConnectionString));
        builder.Services.AddHangfireServer();
        builder.Services.AddScoped<RefreshTokenCleanupJob>();

        hangfireOperational = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hangfire disabled: Redis is unavailable. {ex.Message}");
    }
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

if (hangfireOperational)
{
    app.UseHangfireDashboard("/hangfire");

    try
    {
        RecurringJob.AddOrUpdate<RefreshTokenCleanupJob>(
            "cleanup-expired-refresh-tokens",
            job => job.CleanupExpiredTokensAsync(),
            Cron.Daily);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Failed to register Hangfire recurring cleanup job");
    }
}

app.MapControllers();

await app.RunAsync();

static string EnsureAbortConnectFalse(string connectionString)
{
    if (connectionString.Contains("abortConnect=", StringComparison.OrdinalIgnoreCase) ||
        connectionString.Contains("AbortOnConnectFail=", StringComparison.OrdinalIgnoreCase))
    {
        return connectionString;
    }

    return $"{connectionString},abortConnect=false";
}
