using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Enums.Vendors;

namespace Yooocan.Web.Migrations
{
    public partial class AddedColumnsToVendorMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessType",
                table: "Vendors",
                nullable: false,
                defaultValue: VendorBusinessType.Other);

            migrationBuilder.AddColumn<string>(
                name: "BusinessTypeOther",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialTermOther",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialTermRate",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommercialTerms",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonEmail",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonName",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonPhoneNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonPosition",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonSkype",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationText",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnBoardingDate",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TollFreeNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Vendors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BusinessTypeOther",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CommercialTermOther",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CommercialTermRate",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CommercialTerms",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPersonEmail",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPersonName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPersonPhoneNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPersonPosition",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactPersonSkype",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "LocationText",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "OnBoardingDate",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TelephoneNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TollFreeNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Vendors");
        }
    }
}
