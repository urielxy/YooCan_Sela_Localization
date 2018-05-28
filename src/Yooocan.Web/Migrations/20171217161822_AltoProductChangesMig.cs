using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Yooocan.Web.Migrations
{
    public partial class AltoProductChangesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountRate",
                table: "Products",
                type: "decimal(18, 2)",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons",
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
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCategory_Companies_CompanyId",
                table: "CompanyCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyContactPersons_Companies_CompanyId",
                table: "CompanyContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyShippings_Companies_CompanyId",
                table: "CompanyShippings");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "ProductCategories");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Products",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountRate",
                table: "Products",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BenefitCategories_Benefits_BenefitId",
                table: "BenefitCategories",
                column: "BenefitId",
                principalTable: "Benefits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_CompanyCoupons_Companies_CompanyId",
                table: "CompanyCoupons",
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
