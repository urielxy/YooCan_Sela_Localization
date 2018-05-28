using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class RenamedCommercialTermsColumnsInVendorMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialTermOther",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CommercialTermRate",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "CommercialTermsOther",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialTermsRate",
                table: "Vendors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialTermsOther",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CommercialTermsRate",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "CommercialTermOther",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialTermRate",
                table: "Vendors",
                nullable: true);
        }
    }
}
