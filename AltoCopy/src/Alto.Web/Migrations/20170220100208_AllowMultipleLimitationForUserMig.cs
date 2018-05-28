using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class AllowMultipleLimitationForUserMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Limitations_LimitationId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LimitationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LimitationId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserLimitation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeleteDate = table.Column<DateTime>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    LimitationId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLimitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLimitation_Limitations_LimitationId",
                        column: x => x.LimitationId,
                        principalTable: "Limitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLimitation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLimitation_LimitationId",
                table: "UserLimitation",
                column: "LimitationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLimitation_UserId",
                table: "UserLimitation",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLimitation");

            migrationBuilder.AddColumn<int>(
                name: "LimitationId",
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
    }
}
