using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class RenameStoryCompetitionColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsReelAbilitiesCompetition",
                table: "Stories",
                newName: "IsInCompetition");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsInCompetition",
                table: "Stories",
                newName: "IsReelAbilitiesCompetition");
        }
    }
}
