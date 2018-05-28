using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class LocationForActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityLocation",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLocation",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stories");
        }
    }
}
