using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddFewFieldsToProductMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSoldOnSite",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "IsSoldOnSite",
                table: "Products");
        }
    }
}
