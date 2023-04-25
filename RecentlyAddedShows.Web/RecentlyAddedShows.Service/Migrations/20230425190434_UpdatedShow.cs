using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecentlyAddedShows.Service.Migrations
{
    public partial class UpdatedShow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Shows",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Shows");
        }
    }
}
