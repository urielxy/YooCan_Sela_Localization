using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class AddReferralsTablesMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenefitReferrals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BenefitId = table.Column<int>(nullable: false),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Ip = table.Column<string>(nullable: false),
                    Referrer = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenefitReferrals_Benefits_BenefitId",
                        column: x => x.BenefitId,
                        principalTable: "Benefits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BenefitReferrals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    UserAgent = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_BenefitReferrals_BenefitId",
                table: "BenefitReferrals",
                column: "BenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitReferrals_UserId",
                table: "BenefitReferrals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferrals_ProductId",
                table: "ProductReferrals",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferrals_UserId",
                table: "ProductReferrals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenefitReferrals");

            migrationBuilder.DropTable(
                name: "ProductReferrals");
        }
    }
}
