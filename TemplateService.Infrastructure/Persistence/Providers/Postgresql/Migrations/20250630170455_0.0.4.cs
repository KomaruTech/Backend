using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_notifications",
                schema: "DEFAULT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_notifications_events_EventId",
                        column: x => x.EventId,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_notifications_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_notifications_EventId",
                schema: "DEFAULT",
                table: "event_notifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_notifications_UserId_EventId_NotificationType",
                schema: "DEFAULT",
                table: "event_notifications",
                columns: new[] { "UserId", "EventId", "NotificationType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_notifications",
                schema: "DEFAULT");
        }
    }
}
