using Message.Message.Domain;
using Microsoft.EntityFrameworkCore;

namespace Message.Message.Infrastructure;

internal sealed class ChatMessageRepository : IChatMessageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ChatMessageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Messages
            .OrderBy(m => m.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(ChatMessage message)
    {
        _dbContext.Messages.Add(message);
    }
}
