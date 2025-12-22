using Microsoft.EntityFrameworkCore;
using Pingo.Message.Domain;
using Pingo.Messages.Domain;
using Pingo.Messages.Infrastructure.Database;

namespace Pingo.Messeges.Infrastructure.DataBase;

public sealed class MessagesDbContext(DbContextOptions<MessagesDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<ChatMessage> Messages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Messages);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
    }
}
