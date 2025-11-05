using Message.Message.Domain;
using Microsoft.EntityFrameworkCore;

namespace Message.Message.Infrastructure;

internal sealed class ChatMessageRepository(ApplicationDbContext dbContext) : IChatMessageRepository
{
    public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Messages
            .OrderBy(m => m.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(ChatMessage message)
    {
        dbContext.Messages.Add(message);
    }
}
