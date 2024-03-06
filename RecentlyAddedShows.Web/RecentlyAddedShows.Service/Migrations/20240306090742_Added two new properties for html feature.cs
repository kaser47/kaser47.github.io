using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecentlyAddedShows.Service.Migrations
{
    public partial class Addedtwonewpropertiesforhtmlfeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInHtml",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ShowInHtml",
                table: "Shows");
        }
    }
}
