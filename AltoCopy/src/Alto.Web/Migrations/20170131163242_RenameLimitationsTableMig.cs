using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class RenameLimitationsTableMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limitation_Limitation_ParentLimitationId",
                table: "Limitation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Limitation_LimitationId",
                table: "ProductLimitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Limitation",
                table: "Limitation");

            migrationBuilder.RenameTable(
                name: "Limitation",
                newName: "Limitations");

            migrationBuilder.RenameIndex(
                name: "IX_Limitation_ParentLimitationId",
                table: "Limitations",
                newName: "IX_Limitations_ParentLimitationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Limitations",
                table: "Limitations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CdnUrl = table.Column<string>(nullable: false),
                    DeleteDate = table.Column<DateTime>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserImages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_UserId",
                table: "UserImages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Limitations_Limitations_ParentLimitationId",
                table: "Limitations",
                column: "ParentLimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limitations_Limitations_ParentLimitationId",
                table: "Limitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations");

            migrationBuilder.DropTable(
                name: "UserImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Limitations",
                table: "Limitations");

            migrationBuilder.RenameTable(
                name: "Limitations",
                newName: "Limitation");

            migrationBuilder.RenameIndex(
                name: "IX_Limitations_ParentLimitationId",
                table: "Limitation",
                newName: "IX_Limitation_ParentLimitationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Limitation",
                table: "Limitation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Limitation_Limitation_ParentLimitationId",
                table: "Limitation",
                column: "ParentLimitationId",
                principalTable: "Limitation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Limitation_LimitationId",
                table: "ProductLimitations",
                column: "LimitationId",
                principalTable: "Limitation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
