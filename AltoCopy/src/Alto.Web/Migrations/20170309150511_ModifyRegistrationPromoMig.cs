using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class ModifyRegistrationPromoMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "RegistrationPromos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "RegistrationPromos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "RegistrationPromos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "RegistrationPromos");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "RegistrationPromos");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "RegistrationPromos");

            migrationBuilder.AddForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
