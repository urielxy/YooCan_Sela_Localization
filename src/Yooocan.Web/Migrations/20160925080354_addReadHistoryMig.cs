using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class addReadHistoryMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    StoryId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Stories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReadHistories_UserId",
                table: "ReadHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadHistories_UserId_CategoryId_StoryId_InsertDate",
                table: "ReadHistories",
                columns: new[] { "UserId", "InsertDate", "CategoryId", "StoryId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "ReadHistories");
        }
    }
}
