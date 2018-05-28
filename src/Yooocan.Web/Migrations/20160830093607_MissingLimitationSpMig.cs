using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class MissingLimitationSpMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceProviderCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    ServiceProviderId = table.Column<int>(nullable: false),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderCategories", x => new { x.CategoryId, x.ServiceProviderId });
                    table.ForeignKey(
                        name: "FK_ServiceProviderCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceProviderCategories_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderLimitations",
                columns: table => new
                {
                    LimitationId = table.Column<int>(nullable: false),
                    ServiceProviderId = table.Column<int>(nullable: false),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderLimitations", x => new { x.LimitationId, x.ServiceProviderId });
                    table.ForeignKey(
                        name: "FK_ServiceProviderLimitations_Limitations_LimitationId",
                        column: x => x.LimitationId,
                        principalTable: "Limitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceProviderLimitations_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderCategories_CategoryId",
                table: "ServiceProviderCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderCategories_ServiceProviderId",
                table: "ServiceProviderCategories",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderLimitations_LimitationId",
                table: "ServiceProviderLimitations",
                column: "LimitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderLimitations_ServiceProviderId",
                table: "ServiceProviderLimitations",
                column: "ServiceProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceProviderCategories");

            migrationBuilder.DropTable(
                name: "ServiceProviderLimitations");
        }
    }
}
