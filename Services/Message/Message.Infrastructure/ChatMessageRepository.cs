using Message.Message.Domain;
using Microsoft.EntityFrameworkCore;

namespace Message.Message.Infrastructure;

internal sealed class ChatMessageRepository(ApplicationDbContext dbContext) : IChatMessageRepository
{
    public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Messages
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(ChatMessage message)
    {
        dbContext.Messages.Add(message);
    }

    public async Task UpsertAsync(ChatMessage message, CancellationToken ct = default)
    {
        var existing = await dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == message.Id, ct);

        if (existing == null)
        {
            dbContext.Messages.Add(message);
        }
        else
        {
            existing.Update(message.Content, DateTimeOffset.UtcNow);
        }
    }
}
