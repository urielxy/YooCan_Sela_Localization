using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class AddFewCompanyRelatedStuffMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessType",
                table: "Companies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BusinessTypeOther",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Companies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationText",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MembersDiscountRate",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnBoardingContactPersonEmail",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnBoardingDate",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateType",
                table: "Companies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ReferralRate",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumber",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TollFreeNumber",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Companies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContactPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    ContactPersonEmail = table.Column<string>(nullable: false),
                    ContactPersonName = table.Column<string>(nullable: true),
                    ContactPersonPhoneNumber = table.Column<string>(nullable: true),
                    ContactPersonPosition = table.Column<string>(nullable: true),
                    ContactPersonSkype = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyContactPersons_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCategory_CategoryId",
                table: "CompanyCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCategory_CompanyId",
                table: "CompanyCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContactPersons_CompanyId",
                table: "CompanyContactPersons",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyCategory");

            migrationBuilder.DropTable(
                name: "CompanyContactPersons");

            migrationBuilder.DropColumn(
                name: "About",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BusinessTypeOther",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "LocationText",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MembersDiscountRate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OnBoardingContactPersonEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OnBoardingDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RateType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ReferralRate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TelephoneNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TollFreeNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Companies");
        }
    }
}
