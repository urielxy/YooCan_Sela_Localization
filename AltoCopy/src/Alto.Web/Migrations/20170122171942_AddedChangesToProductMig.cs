using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddedChangesToProductMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "About",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Specifications",
                table: "Products",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "ProductImage",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Products",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "Specifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductImage",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "ProductImage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Products",
                nullable: true);
        }
    }
}
