using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class ImportsYoocanDbMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Products");

            migrationBuilder.EnsureSchema(
                name: "Imports");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vendors",
                schema: "Imports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    About = table.Column<string>(nullable: true),
                    BusinessType = table.Column<int>(nullable: false),
                    BusinessTypeOther = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CommercialTerms = table.Column<int>(nullable: true),
                    CommercialTermsOther = table.Column<string>(nullable: true),
                    CommercialTermsRate = table.Column<decimal>(nullable: true),
                    CompanyCode = table.Column<string>(nullable: true),
                    ContactPersonEmail = table.Column<string>(nullable: true),
                    ContactPersonName = table.Column<string>(nullable: true),
                    ContactPersonPhoneNumber = table.Column<string>(nullable: true),
                    ContactPersonPosition = table.Column<string>(nullable: true),
                    ContactPersonSkype = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    FaxNumber = table.Column<string>(nullable: true),
                    GooglePlaceId = table.Column<string>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Instagram = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    LocationText = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OnBoardingContactPersonEmail = table.Column<string>(nullable: true),
                    OnBoardingDate = table.Column<DateTime>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true),
                    TelephoneNumber = table.Column<string>(nullable: true),
                    TollFreeNumber = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    WebsiteUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Imports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    About = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Colors = table.Column<string>(nullable: true),
                    Depth = table.Column<float>(nullable: true),
                    Height = table.Column<float>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    ListPrice = table.Column<decimal>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Specifications = table.Column<string>(nullable: true),
                    Upc = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    VideoUrl = table.Column<string>(nullable: true),
                    WarrentyUrl = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    Width = table.Column<float>(nullable: true),
                    YouTubeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Imports",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                schema: "Imports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CdnUrl = table.Column<string>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    OriginalUrl = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Imports",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId",
                schema: "Imports",
                table: "Products",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                schema: "Imports",
                table: "ProductImages",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages",
                schema: "Imports");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Imports");

            migrationBuilder.DropTable(
                name: "Vendors",
                schema: "Imports");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }
    }
}
