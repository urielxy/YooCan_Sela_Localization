using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class ChangesToVendorRegisterationMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_Vendors_VendorId",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_VendorId",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "CommercialMode",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "FreeShippingOver",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "InterestedInAds",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "InterestedInSponsorship",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "MegaEcommercePlatforms",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "NumberOfProducts",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "OwnEcommerceWebsite",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "SpecializedDealerEommerceWebsite",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "TollFreePhoneNumber",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "VendorRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "ContactPresonRole",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "VendorRegistrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPresonRole",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "VendorRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommercialMode",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FreeShippingOver",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InterestedInAds",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InterestedInSponsorship",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MegaEcommercePlatforms",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfProducts",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OwnEcommerceWebsite",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ShippingCost",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SpecializedDealerEommerceWebsite",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TollFreePhoneNumber",
                table: "VendorRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "VendorRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_VendorId",
                table: "VendorRegistrations",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_Vendors_VendorId",
                table: "VendorRegistrations",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
