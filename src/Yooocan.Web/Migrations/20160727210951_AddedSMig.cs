using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddedSMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryImage_Stories_StoryId",
                table: "StoryImage");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryParagraph_Stories_StoryId",
                table: "StoryParagraph");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProduct_Products_ProductId",
                table: "StoryProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProduct_Stories_StoryId",
                table: "StoryProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryProduct",
                table: "StoryProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryParagraph",
                table: "StoryParagraph");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryImage",
                table: "StoryImage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryProducts",
                table: "StoryProduct",
                columns: new[] { "StoryId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryParagraphs",
                table: "StoryParagraph",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryImages",
                table: "StoryImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImage",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraph",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProduct",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.RenameIndex(
                name: "IX_StoryProduct_StoryId",
                table: "StoryProduct",
                newName: "IX_StoryProducts_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryProduct_ProductId",
                table: "StoryProduct",
                newName: "IX_StoryProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryParagraph_StoryId",
                table: "StoryParagraph",
                newName: "IX_StoryParagraphs_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryImage_StoryId",
                table: "StoryImage",
                newName: "IX_StoryImages_StoryId");

            migrationBuilder.RenameTable(
                name: "StoryProduct",
                newName: "StoryProducts");

            migrationBuilder.RenameTable(
                name: "StoryParagraph",
                newName: "StoryParagraphs");

            migrationBuilder.RenameTable(
                name: "StoryImage",
                newName: "StoryImages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraphs");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryProducts",
                table: "StoryProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryParagraphs",
                table: "StoryParagraphs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryImages",
                table: "StoryImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryProduct",
                table: "StoryProducts",
                columns: new[] { "StoryId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryParagraph",
                table: "StoryParagraphs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryImage",
                table: "StoryImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryImage_Stories_StoryId",
                table: "StoryImages",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryParagraph_Stories_StoryId",
                table: "StoryParagraphs",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProduct_Products_ProductId",
                table: "StoryProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProduct_Stories_StoryId",
                table: "StoryProducts",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.RenameIndex(
                name: "IX_StoryProducts_StoryId",
                table: "StoryProducts",
                newName: "IX_StoryProduct_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryProducts_ProductId",
                table: "StoryProducts",
                newName: "IX_StoryProduct_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryParagraphs_StoryId",
                table: "StoryParagraphs",
                newName: "IX_StoryParagraph_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryImages_StoryId",
                table: "StoryImages",
                newName: "IX_StoryImage_StoryId");

            migrationBuilder.RenameTable(
                name: "StoryProducts",
                newName: "StoryProduct");

            migrationBuilder.RenameTable(
                name: "StoryParagraphs",
                newName: "StoryParagraph");

            migrationBuilder.RenameTable(
                name: "StoryImages",
                newName: "StoryImage");
        }
    }
}
