using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class SetDeleteCascadeOnCompanyCollectionsMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
