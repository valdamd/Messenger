using Microsoft.EntityFrameworkCore.Migrations;

namespace Pingo.Messages.Infrastructure.DataBase;

public sealed class CreateMessagesTable : Migration
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
                row_version = table.Column<byte[]>(rowVersion: true, nullable: false),
            },
            schema: "public",
            constraints: table => table.PrimaryKey("pk_messages", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_messages_created_at",
            table: "messages",
            schema: "public",
            column: "created_at");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "messages",
            schema: "public");
    }
}
