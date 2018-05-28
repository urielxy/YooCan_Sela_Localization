using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class AddLimitationFollowingMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LimitationFollowers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeleteDate = table.Column<DateTime>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    LimitationId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitationFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimitationFollowers_Limitations_LimitationId",
                        column: x => x.LimitationId,
                        principalTable: "Limitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LimitationFollowers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<bool>(
                name: "CustomizedFeedDone",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_LimitationFollowers_LimitationId",
                table: "LimitationFollowers",
                column: "LimitationId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitationFollowers_UserId",
                table: "LimitationFollowers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitationFollowers_UserId_LimitationId_DeleteDate",
                table: "LimitationFollowers",
                columns: new[] { "UserId", "LimitationId", "DeleteDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomizedFeedDone",
                table: "Users");

            migrationBuilder.DropTable(
                name: "LimitationFollowers");
        }
    }
}
