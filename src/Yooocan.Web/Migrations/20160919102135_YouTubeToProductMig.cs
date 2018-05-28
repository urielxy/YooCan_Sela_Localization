using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class YouTubeToProductMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeId",
                table: "Products");
        }
    }
}
