using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class ShippingChangesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShipping_Companies_CompanyId",
                table: "CompanyShipping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyShipping",
                table: "CompanyShipping");

            migrationBuilder.RenameTable(
                name: "CompanyShipping",
                newName: "CompanyShippings");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyShipping_CompanyId",
                table: "CompanyShippings",
                newName: "IX_CompanyShippings_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyShippings",
                table: "CompanyShippings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductShippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    MaxShippingDuration = table.Column<int>(nullable: true),
                    MinShippingDuration = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    ShippingMethod = table.Column<string>(nullable: true),
                    ShippingPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShippings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductShippings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductShippings_ProductId",
                table: "ProductShippings",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings");

            migrationBuilder.DropTable(
                name: "ProductShippings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyShippings",
                table: "CompanyShippings");

            migrationBuilder.RenameTable(
                name: "CompanyShippings",
                newName: "CompanyShipping");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyShippings_CompanyId",
                table: "CompanyShipping",
                newName: "IX_CompanyShipping_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyShipping",
                table: "CompanyShipping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyShipping_Companies_CompanyId",
                table: "CompanyShipping",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
