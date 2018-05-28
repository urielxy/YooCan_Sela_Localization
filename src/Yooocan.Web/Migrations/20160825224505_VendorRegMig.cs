using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yooocan.Web.Migrations
{
    public partial class VendorRegMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    BusinessType = table.Column<int>(nullable: false),
                    BusinessTypeOther = table.Column<string>(nullable: true),
                    CommercialMode = table.Column<int>(nullable: false),
                    ContactPresonName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    FreeShippingOver = table.Column<decimal>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    InterestedInAds = table.Column<bool>(nullable: false),
                    InterestedInSponsorship = table.Column<bool>(nullable: false),
                    MegaEcommercePlatforms = table.Column<bool>(nullable: false),
                    MobileNumber = table.Column<string>(nullable: true),
                    NumberOfProducts = table.Column<int>(nullable: false),
                    OwnEcommerceWebsite = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ShippingCost = table.Column<int>(nullable: false),
                    SpecializedDealerEommerceWebsite = table.Column<bool>(nullable: false),
                    TollFreePhoneNumber = table.Column<string>(nullable: true),
                    VendorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorRegistrations_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_VendorId",
                table: "VendorRegistrations",
                column: "VendorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorRegistrations");
        }
    }
}
