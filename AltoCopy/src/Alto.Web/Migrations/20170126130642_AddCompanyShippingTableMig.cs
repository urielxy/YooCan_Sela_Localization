using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class AddCompanyShippingTableMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyShipping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    MaxProductPrice = table.Column<decimal>(nullable: false),
                    MaxShippingDuration = table.Column<int>(nullable: true),
                    MinProductPrice = table.Column<decimal>(nullable: false),
                    MinShippingDuration = table.Column<int>(nullable: true),
                    ShippingMethod = table.Column<string>(nullable: true),
                    ShippingPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyShipping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyShipping_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyShipping_CompanyId",
                table: "CompanyShipping",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyShipping");
        }
    }
}
