using Microsoft.EntityFrameworkCore;
using Pingo.Messages.Infrastructure.DataBase;

namespace Message.Api.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MessagesDbContext>();

        dbContext.Database.Migrate();
    }
}
