using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Domain;

namespace Pingo.Messages.Infrastructure.DataBase;

public sealed class MessagesDbContext(DbContextOptions<MessagesDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
    }
}
