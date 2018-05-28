using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Enums;

namespace Yooocan.Web.Migrations
{
    public partial class AddDiscountFieldsToProductMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRate",
                table: "Products",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Products",
                nullable: false,
                defaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Products");
        }
    }
}
