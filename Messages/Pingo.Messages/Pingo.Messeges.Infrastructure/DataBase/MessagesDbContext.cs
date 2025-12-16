using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Models;

namespace Pingo.Messages.Infrastructure.Database;

public class MessagesDbContext(DbContextOptions<MessagesDbContext> options, DbSet<Message> messages) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; } = messages;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages", "public");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Text)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(m => m.CreatedAt)
                .IsRequired();

            entity.Property(m => m.UpdatedAt)
                .IsRequired(false);
        });
    }
}
