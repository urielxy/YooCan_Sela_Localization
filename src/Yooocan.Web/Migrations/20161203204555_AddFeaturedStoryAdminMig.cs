
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class AddFeaturedStoryAdminMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeaturedStories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FeaturedType = table.Column<int>(nullable: false),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    StoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturedStories_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<bool>(
                name: "CanNotBeFeaturedOnHomePage",
                table: "Stories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedStories_StoryId",
                table: "FeaturedStories",
                column: "StoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanNotBeFeaturedOnHomePage",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "FeaturedStories");
        }
    }
}
