using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Enums;

namespace Yooocan.Web.Migrations
{
    public partial class AddMoreFieldsToProductMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProductImages",
                nullable: false,
                defaultValue: ImageType.Normal);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Upc",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarrentyUrl",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Upc",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WarrentyUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Products");
        }
    }
}
