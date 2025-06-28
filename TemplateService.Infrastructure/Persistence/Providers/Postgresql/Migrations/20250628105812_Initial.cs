using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DEFAULT");

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    notify_telegram = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notify_email = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    reminder_before_1day = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    reminder_before_1hour = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_preferences", x => x.id);
                },
                comment: "Предпочтения пользователей");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "member"),
                    telegram_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notification_preferences_id = table.Column<Guid>(type: "uuid", nullable: false),
                    avatar = table.Column<byte[]>(type: "bytea", nullable: true),
                    avatar_mime_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_notification_preferences_notification_preferences_id",
                        column: x => x.notification_preferences_id,
                        principalSchema: "DEFAULT",
                        principalTable: "notification_preferences",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Пользователи");

            migrationBuilder.CreateTable(
                name: "events",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    time_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "general"),
                    location = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    keywords = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_users_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id");
                },
                comment: "Мероприятия");

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_teams_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Команды (Группы)");

            migrationBuilder.CreateTable(
                name: "event_feedback",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_feedback", x => x.id);
                    table.CheckConstraint("CK_event_feedback_rating_range", "rating BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_event_feedback_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_feedback_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Отзывы на мероприятия");

            migrationBuilder.CreateTable(
                name: "event_participants",
                schema: "DEFAULT",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_speaker = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Флаг, выступает ли пользователь на мероприятии"),
                    attendance_marked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Флаг, был ли пользователь отмечен на мероприятии")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participants", x => new { x.user_id, x.event_id });
                    table.ForeignKey(
                        name: "FK_event_participants_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_participants_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь пользователей с мероприятиями");

            migrationBuilder.CreateTable(
                name: "event_photos",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image = table.Column<byte[]>(type: "bytea", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_photos_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Фотографии, прикреплённые к событиям");

            migrationBuilder.CreateTable(
                name: "speaker_applications",
                schema: "DEFAULT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    comment = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_speaker_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_speaker_applications_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_speaker_applications_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Заявки на выступление");

            migrationBuilder.CreateTable(
                name: "event_groups",
                schema: "DEFAULT",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_groups", x => new { x.event_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_event_groups_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "DEFAULT",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_groups_teams_group_id",
                        column: x => x.group_id,
                        principalSchema: "DEFAULT",
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь мероприятий с группами");

            migrationBuilder.CreateTable(
                name: "user_teams",
                schema: "DEFAULT",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_teams", x => new { x.user_id, x.team_id });
                    table.ForeignKey(
                        name: "FK_user_teams_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "DEFAULT",
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_teams_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "DEFAULT",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь пользователей и команд");

            migrationBuilder.CreateIndex(
                name: "IX_event_feedback_event_id",
                schema: "DEFAULT",
                table: "event_feedback",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_feedback_user_id",
                schema: "DEFAULT",
                table: "event_feedback",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_groups_group_id",
                schema: "DEFAULT",
                table: "event_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_event_id",
                schema: "DEFAULT",
                table: "event_participants",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_photos_event_id",
                schema: "DEFAULT",
                table: "event_photos",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_created_by_id",
                schema: "DEFAULT",
                table: "events",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_speaker_applications_event_id",
                schema: "DEFAULT",
                table: "speaker_applications",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_speaker_applications_user_id",
                schema: "DEFAULT",
                table: "speaker_applications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_name",
                schema: "DEFAULT",
                table: "teams",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_owner_id",
                schema: "DEFAULT",
                table: "teams",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_teams_team_id",
                schema: "DEFAULT",
                table: "user_teams",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_login",
                schema: "DEFAULT",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_notification_preferences_id",
                schema: "DEFAULT",
                table: "users",
                column: "notification_preferences_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_feedback",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "event_groups",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "event_participants",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "event_photos",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "speaker_applications",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "user_teams",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "events",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "users",
                schema: "DEFAULT");

            migrationBuilder.DropTable(
                name: "notification_preferences",
                schema: "DEFAULT");
        }
    }
}
