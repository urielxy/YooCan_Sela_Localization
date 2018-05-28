using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class IndexToStoryPublishDateMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Stories_Id_PublishDate_IsPublished",
                table: "Stories",
                columns: new[] { "Id", "PublishDate", "IsPublished" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stories_Id_PublishDate_IsPublished",
                table: "Stories");
        }
    }
}
