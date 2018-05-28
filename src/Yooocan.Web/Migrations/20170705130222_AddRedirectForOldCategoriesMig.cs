using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddRedirectForOldCategoriesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RedirectCategoryId",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_RedirectCategoryId",
                table: "Categories",
                column: "RedirectCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_RedirectCategoryId",
                table: "Categories",
                column: "RedirectCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_RedirectCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_RedirectCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "RedirectCategoryId",
                table: "Categories");
        }
    }
}
