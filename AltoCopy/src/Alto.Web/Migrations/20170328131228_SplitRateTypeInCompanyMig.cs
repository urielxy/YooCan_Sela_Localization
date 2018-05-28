using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class SplitRateTypeInCompanyMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "RateType",
                newName: "DiscountRateType",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "ReferralRateType",
                table: "Companies",
                nullable: true);

            migrationBuilder.Sql("Update Companies set ReferralRateType = DiscountRateType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferralRateType",
                table: "Companies");

            migrationBuilder.RenameColumn(
                "DiscountRateType",
                newName: "RateType",
                table: "Companies");
        }
    }
}
