using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class AddRegisterPromoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferralPromoId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrationPromos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyLogo = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    PromoCode = table.Column<string>(nullable: true),
                    ReferralsRegistered = table.Column<int>(nullable: false),
                    ReferralsRemaining = table.Column<int>(nullable: true),
                    ReferrerUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationPromos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationPromos_Users_ReferrerUserId",
                        column: x => x.ReferrerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReferralPromoId",
                table: "Users",
                column: "ReferralPromoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPromos_ReferrerUserId",
                table: "RegistrationPromos",
                column: "ReferrerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RegistrationPromos_ReferralPromoId",
                table: "Users",
                column: "ReferralPromoId",
                principalTable: "RegistrationPromos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RegistrationPromos_ReferralPromoId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RegistrationPromos");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReferralPromoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferralPromoId",
                table: "Users");
        }
    }
}
