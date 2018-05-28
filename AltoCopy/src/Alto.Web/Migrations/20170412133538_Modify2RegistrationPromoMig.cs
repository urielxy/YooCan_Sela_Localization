using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class Modify2RegistrationPromoMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RegistrationPromos",
                type: "decimal(18, 9)",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotOrganization",
                table: "RegistrationPromos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotOrganization",
                table: "RegistrationPromos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RegistrationPromos",
                nullable: false);
        }
    }
}
