using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class RemoveRequiredStuffFromOrderMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SaleId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                table: "Orders",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Orders",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "SaleId",
                table: "Orders",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Orders",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Orders",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                table: "Orders",
                nullable: false);
        }
    }
}
