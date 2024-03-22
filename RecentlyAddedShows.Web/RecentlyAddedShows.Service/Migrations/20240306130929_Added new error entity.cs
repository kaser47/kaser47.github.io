using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecentlyAddedShows.Service.Migrations
{
    public partial class Addednewerrorentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorDetailsid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_ErrorDetails_ErrorDetails_ErrorDetailsid",
                        column: x => x.ErrorDetailsid,
                        principalTable: "ErrorDetails",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorDetails_ErrorDetailsid",
                table: "ErrorDetails",
                column: "ErrorDetailsid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorDetails");
        }
    }
}
