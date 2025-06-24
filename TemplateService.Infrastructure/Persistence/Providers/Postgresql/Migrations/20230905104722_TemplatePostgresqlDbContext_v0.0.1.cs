using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class TemplatePostgresqlDbContext_v001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "XXATACH_TMP");

            migrationBuilder.CreateTable(
                name: "TMP_Documents",
                schema: "XXATACH_TMP",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMP_Documents", x => x.Id);
                },
                comment: "Документы");

            migrationBuilder.CreateTable(
                name: "TMP_MetaTypes",
                schema: "XXATACH_TMP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMP_MetaTypes", x => x.Id);
                },
                comment: "Тип мета-данных");

            migrationBuilder.CreateTable(
                name: "TMP_Metas",
                schema: "XXATACH_TMP",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MetaTypeId = table.Column<int>(type: "integer", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMP_Metas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TMP_Metas_TMP_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "XXATACH_TMP",
                        principalTable: "TMP_Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TMP_Metas_TMP_MetaTypes_MetaTypeId",
                        column: x => x.MetaTypeId,
                        principalSchema: "XXATACH_TMP",
                        principalTable: "TMP_MetaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Мета-описание документа");

            migrationBuilder.InsertData(
                schema: "XXATACH_TMP",
                table: "TMP_MetaTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "ID документа в СЭД АТАЧ", "atachdocumentid" },
                    { 2, "Регистрационный номер", "regnumber" },
                    { 3, "Дата регистрации", "regdate" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TMP_Documents_Number",
                schema: "XXATACH_TMP",
                table: "TMP_Documents",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TMP_Metas_DocumentId",
                schema: "XXATACH_TMP",
                table: "TMP_Metas",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TMP_Metas_MetaTypeId",
                schema: "XXATACH_TMP",
                table: "TMP_Metas",
                column: "MetaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TMP_MetaTypes_Name",
                schema: "XXATACH_TMP",
                table: "TMP_MetaTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TMP_Metas",
                schema: "XXATACH_TMP");

            migrationBuilder.DropTable(
                name: "TMP_Documents",
                schema: "XXATACH_TMP");

            migrationBuilder.DropTable(
                name: "TMP_MetaTypes",
                schema: "XXATACH_TMP");
        }
    }
}
