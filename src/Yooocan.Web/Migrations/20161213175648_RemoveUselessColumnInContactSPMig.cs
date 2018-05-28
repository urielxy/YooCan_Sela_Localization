using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class RemoveUselessColumnInContactSPMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId1",
                table: "ServiceProviderContactRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderContactRequests_ServiceProviderId1",
                table: "ServiceProviderContactRequests");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId1",
                table: "ServiceProviderContactRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceProviderId",
                table: "ServiceProviderContactRequests",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderContactRequests_ServiceProviderId",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderContactRequests_ServiceProviderId",
                table: "ServiceProviderContactRequests");

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId1",
                table: "ServiceProviderContactRequests",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceProviderId",
                table: "ServiceProviderContactRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderContactRequests_ServiceProviderId1",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId1",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId1",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
