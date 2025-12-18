using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Application;
using Pingo.Messages.Presentation;
using Pingo.Messages.Infrastructure.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Добавьте конфигурацию
builder.Services.AddDbContext<MessagesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MessagesDb"))); // PostgreSQL провайдер

// Зарегистрируйте репозиторий и сервис
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<MessageService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Для Swagger, если нужно

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Примените миграции
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MessagesDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.MapEndpoints(); // Вызов эндпоинтов

app.Run();
