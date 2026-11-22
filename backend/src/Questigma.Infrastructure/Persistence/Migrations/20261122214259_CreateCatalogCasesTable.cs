using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Questigma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateCatalogCasesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "catalog_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    synopsis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    introduction = table.Column<string>(type: "text", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_cases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_cases_is_published",
                table: "catalog_cases",
                column: "is_published");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalog_cases");
        }
    }
}
