using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class ServiceProviderActivityChangeMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceProviderLocations");

            migrationBuilder.CreateTable(
                name: "ServiceProviderActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    OpenDays = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    ServiceProviderId = table.Column<int>(nullable: true),
                    Units = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProviderActivities_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "ServiceProviders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "ServiceProviders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChapter",
                table: "ServiceProviders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "ServiceProviders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TollFreePhoneNumber",
                table: "ServiceProviders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderActivities_ServiceProviderId",
                table: "ServiceProviderActivities",
                column: "ServiceProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsChapter",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "TollFreePhoneNumber",
                table: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "ServiceProviderActivities");

            migrationBuilder.CreateTable(
                name: "ServiceProviderLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    ServiceProviderId = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProviderLocations_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderLocations_ServiceProviderId",
                table: "ServiceProviderLocations",
                column: "ServiceProviderId");
        }
    }
}
