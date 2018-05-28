using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddDetailsToUserMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountRelationship",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitationId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LimitationOther",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZipCode",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_LimitationId",
                table: "Users",
                column: "LimitationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Limitations_LimitationId",
                table: "Users",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Limitations_LimitationId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LimitationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountRelationship",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LimitationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LimitationOther",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Users");
        }
    }
}
