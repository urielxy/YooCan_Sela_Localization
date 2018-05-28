using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class StoryMoreLocationMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Stories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Stories");
        }
    }
}
