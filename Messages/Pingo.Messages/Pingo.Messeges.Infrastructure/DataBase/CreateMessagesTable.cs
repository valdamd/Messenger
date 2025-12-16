using Microsoft.EntityFrameworkCore.Migrations;

namespace Pingo.Messages.Infrastructure.Database;

public class CreateMessagesTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "messages",
            columns: table => new
            {
                id = table.Column<Guid>(nullable: false),
                text = table.Column<string>(maxLength: 1000, nullable: false),
                created_at = table.Column<DateTimeOffset>(nullable: false),
                updated_at = table.Column<DateTimeOffset>(nullable: true),
            },
            schema: "public",
            constraints: table => table.PrimaryKey("pk_messages", x => x.id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "messages",
            schema: "public");
    }
}
