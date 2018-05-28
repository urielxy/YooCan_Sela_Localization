using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class CompanyTableChangesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPersonEmail",
                table: "CompanyContactPersons");

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
                name: "PostalCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "ContactPersonSkype",
                table: "CompanyContactPersons",
                newName: "Skype");

            migrationBuilder.RenameColumn(
                name: "ContactPersonPosition",
                table: "CompanyContactPersons",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "ContactPersonPhoneNumber",
                table: "CompanyContactPersons",
                newName: "PhoneExtension");

            migrationBuilder.RenameColumn(
                name: "ContactPersonName",
                table: "CompanyContactPersons",
                newName: "MobileNumber");

            migrationBuilder.RenameColumn(
                name: "StreetNumber",
                table: "Companies",
                newName: "OtherRequestedCategories");

            migrationBuilder.RenameColumn(
                name: "StreetName",
                table: "Companies",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CompanyContactPersons",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CompanyContactPersons",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "RateType",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "CompanyContactPersons");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CompanyContactPersons");

            migrationBuilder.RenameColumn(
                name: "Skype",
                table: "CompanyContactPersons",
                newName: "ContactPersonSkype");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "CompanyContactPersons",
                newName: "ContactPersonPosition");

            migrationBuilder.RenameColumn(
                name: "PhoneExtension",
                table: "CompanyContactPersons",
                newName: "ContactPersonPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "CompanyContactPersons",
                newName: "ContactPersonName");

            migrationBuilder.RenameColumn(
                name: "OtherRequestedCategories",
                table: "Companies",
                newName: "StreetNumber");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "StreetName");

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonEmail",
                table: "CompanyContactPersons",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RateType",
                table: "Companies",
                nullable: false);

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

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Companies",
                nullable: true);
        }
    }
}
