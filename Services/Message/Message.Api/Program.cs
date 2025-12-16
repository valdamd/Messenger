using Pingo.Messages.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                 ?? "Host=localhost;Port=5432;Database=messenger_db;Username=postgres;Password=example";

builder.Services.AddControllers();

builder.Services.AddDbContext<MessagesDbContext>(opt => opt.UseNpgsql(connection));

//builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
