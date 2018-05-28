using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddAltoIdColumnToProductsAndVendorsMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AltoId",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AltoId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltoId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AltoId",
                table: "Products");
        }
    }
}
