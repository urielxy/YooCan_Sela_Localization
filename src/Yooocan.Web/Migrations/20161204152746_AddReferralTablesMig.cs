using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class AddReferralTablesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductReferrals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Ip = table.Column<string>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Referrer = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: false),
                    UserAgent = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReferrals_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductReferrals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderReferrals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Ip = table.Column<string>(nullable: false),
                    Referrer = table.Column<string>(nullable: true),
                    ServiceProviderId = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    UserAgent = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProviderReferrals_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceProviderReferrals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferrals_ProductId",
                table: "ProductReferrals",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferrals_UserId",
                table: "ProductReferrals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderReferrals_ServiceProviderId",
                table: "ServiceProviderReferrals",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderReferrals_UserId",
                table: "ServiceProviderReferrals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReferrals");

            migrationBuilder.DropTable(
                name: "ServiceProviderReferrals");
        }
    }
}
