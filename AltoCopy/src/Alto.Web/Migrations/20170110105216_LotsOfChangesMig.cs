using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class LotsOfChangesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Benefits",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Benefits",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Latitude = table.Column<float>(nullable: true),
                    Longitude = table.Column<float>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLocations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Benefits_CompanyId",
                table: "Benefits",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocations_UserId",
                table: "UserLocations",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Benefits_Companies_CompanyId",
                table: "Benefits",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Benefits_Companies_CompanyId",
                table: "Benefits");

            migrationBuilder.DropTable(
                name: "UserLocations");

            migrationBuilder.DropIndex(
                name: "IX_Benefits_CompanyId",
                table: "Benefits");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Benefits");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Benefits");
        }
    }
}
