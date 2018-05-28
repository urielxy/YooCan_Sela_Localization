using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddDateFieldsMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "CompanyShipping",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "CompanyShipping",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "OrderProducts",
                nullable: false,
                defaultValueSql: "GetUtcDate()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "CompanyShipping");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "CompanyShipping");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "OrderProducts");
        }
    }
}
