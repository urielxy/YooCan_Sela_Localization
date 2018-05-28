using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AdditionalInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutActivities",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ServiceProviderActivities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "ServiceProviders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ServiceProviderActivities");

            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "AboutActivities",
                table: "ServiceProviders",
                nullable: true);
        }
    }
}
