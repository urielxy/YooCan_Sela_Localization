using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class LastUpdateDateToStory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "StoryTags",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoryTags",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Stories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "StoryTags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoryTags");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Stories");
        }
    }
}
