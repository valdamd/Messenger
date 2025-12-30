using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pingo.Messeges.Infrastructure.Migrations
{
    /// <inheritdoc />
    public sealed partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messages");

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                },
                schema: "messages",
                constraints: table => table.PrimaryKey("pk_chat_messages", x => x.id));

            migrationBuilder.CreateIndex(
                name: "ix_chat_messages_created_at_utc",
                table: "chat_messages",
                column: "created_at_utc",
                schema: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_messages",
                schema: "messages");
        }
    }
}
