using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddRegistrationRelatedFieldsMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RegistrationPromos_ReferralPromoId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ReferralPromoId",
                table: "Users",
                newName: "ReferrerPromoId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ReferralPromoId",
                table: "Users",
                newName: "IX_Users_ReferrerPromoId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RegistrationPromos_ReferrerPromoId",
                table: "Users",
                column: "ReferrerPromoId",
                principalTable: "RegistrationPromos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RegistrationPromos_ReferrerPromoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ReferrerPromoId",
                table: "Users",
                newName: "ReferralPromoId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ReferrerPromoId",
                table: "Users",
                newName: "IX_Users_ReferralPromoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RegistrationPromos_ReferralPromoId",
                table: "Users",
                column: "ReferralPromoId",
                principalTable: "RegistrationPromos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
