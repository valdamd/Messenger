using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Application;
using Pingo.Messages.Domain;
using Pingo.Messages.Infrastructure.DataBase;

namespace Pingo.Messages.Infrastructure.Repositories;

internal sealed class MessageRepository(MessagesDbContext dbContext) : IMessageRepository
{
    public async Task<Message?> GetAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task SaveAsync(Message message, CancellationToken ct = default)
    {
        if (await dbContext.Messages.AnyAsync(m => m.Id == message.Id, ct))
        {
            dbContext.Messages.Update(message);
        }
        else
        {
            dbContext.Messages.Add(message);
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
