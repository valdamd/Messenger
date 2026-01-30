using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Domain;
using Pingo.Messeges.Infrastructure.DataBase;

namespace Pingo.Messages.Infrastructure;

internal sealed class ChatMessageRepository(MessagesDbContext dbContext) : IChatMessageRepository
{
    public async Task<ChatMessage?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Messages
            .OrderBy(m => m.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Insert(ChatMessage message)
    {
        dbContext.Messages.Add(message);
    }

    public void Update(ChatMessage message)
    {
        dbContext.Messages.Update(message);
    }
}
