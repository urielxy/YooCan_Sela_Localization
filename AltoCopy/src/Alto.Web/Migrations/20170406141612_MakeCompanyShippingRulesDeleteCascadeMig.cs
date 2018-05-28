using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class MakeCompanyShippingRulesDeleteCascadeMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
