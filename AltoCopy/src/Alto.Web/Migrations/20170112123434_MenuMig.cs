using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Web.Migrations
{
    public partial class MenuMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BenefitCategories",
                table: "BenefitCategories");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BenefitCategories",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "BenefitCategories",
                nullable: false,
                defaultValueSql: "GetUtcDate()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BenefitCategories",
                table: "BenefitCategories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    CdnUrl = table.Column<string>(nullable: false),
                    InsertDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GetUtcDate()"),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryImages_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenefitCategories_CategoryId",
                table: "BenefitCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryImages_CategoryId",
                table: "CategoryImages",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BenefitCategories",
                table: "BenefitCategories");

            migrationBuilder.DropIndex(
                name: "IX_BenefitCategories_CategoryId",
                table: "BenefitCategories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BenefitCategories");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "BenefitCategories");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BenefitCategories",
                table: "BenefitCategories",
                columns: new[] { "CategoryId", "BenefitId" });
        }
    }
}
