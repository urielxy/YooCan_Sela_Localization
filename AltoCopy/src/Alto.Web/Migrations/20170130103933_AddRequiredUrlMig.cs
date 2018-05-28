using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddRequiredUrlMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductImages",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CdnUrl",
                table: "ProductImages",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "BenefitImages",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CdnUrl",
                table: "BenefitImages",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CdnUrl",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "BenefitImages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CdnUrl",
                table: "BenefitImages",
                nullable: true);
        }
    }
}
