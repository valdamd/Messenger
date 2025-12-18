using Message.Api.Extensions;
using Message.Api.Middleware;
using Message.Message.Application;
using Message.Message.Infrastructure;
using Message.Message.Infrastructure.Database;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Message.Message.Application.AssemblyReference).Assembly));

var databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;

builder.Services.AddInfrastructure(databaseConnectionString);

builder.Configuration.AddModuleConfiguration(["messages"]);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

await app.RunAsync();
