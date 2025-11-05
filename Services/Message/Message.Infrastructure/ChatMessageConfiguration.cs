using Message.Message.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Message.Message.Infrastructure;

internal sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(message => message.Id);

        builder.Property(message => message.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(message => message.CreatedOnUtc)
            .IsRequired();
    }
}
