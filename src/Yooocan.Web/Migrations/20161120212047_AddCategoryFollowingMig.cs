using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class AddCategoryFollowingMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryFollowers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    DeleteDate = table.Column<DateTime>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryFollowers_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryFollowers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_CategoryFollowers_CategoryId",
                table: "CategoryFollowers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFollowers_UserId",
                table: "CategoryFollowers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFollowers_UserId_CategoryId_DeleteDate",
                table: "CategoryFollowers",
                columns: new[] { "UserId", "CategoryId", "DeleteDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryFollowers");
        }
    }
}
