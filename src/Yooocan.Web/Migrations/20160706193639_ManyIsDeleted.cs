using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class ManyIsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedImageUrl",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "HeaderImageUrl",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "SearchImageUrl",
                table: "Stories");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "StoryLimitations",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoryLimitations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "StoryCategories",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoryCategories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "StoryLimitations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoryLimitations");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "StoryCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoryCategories");

            migrationBuilder.AddColumn<string>(
                name: "FeedImageUrl",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderImageUrl",
                table: "Stories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchImageUrl",
                table: "Stories",
                nullable: true);
        }
    }
}
