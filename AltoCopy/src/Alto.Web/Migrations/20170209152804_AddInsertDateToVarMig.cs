using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddInsertDateToVarMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductShippings_ProductId",
                table: "ProductShippings");

            migrationBuilder.DropColumn(
                name: "ListPrice",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "ProductVariationValues",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "ProductVariationCombinations",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.CreateIndex(
                name: "IX_ProductShippings_ProductId",
                table: "ProductShippings",
                column: "ProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductShippings_ProductId",
                table: "ProductShippings");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "ProductVariationValues");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "ProductVariationCombinations");

            migrationBuilder.AddColumn<decimal>(
                name: "ListPrice",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductShippings_ProductId",
                table: "ProductShippings",
                column: "ProductId");
        }
    }
}
