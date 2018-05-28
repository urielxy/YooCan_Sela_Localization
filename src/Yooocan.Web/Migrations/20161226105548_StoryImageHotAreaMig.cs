using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class StoryImageHotAreaMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "HotAreaLeft",
                table: "StoryImages",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HotAreaTop",
                table: "StoryImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotAreaLeft",
                table: "StoryImages");

            migrationBuilder.DropColumn(
                name: "HotAreaTop",
                table: "StoryImages");
        }
    }
}
