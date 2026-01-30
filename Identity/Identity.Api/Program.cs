using Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddBackgroundServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
