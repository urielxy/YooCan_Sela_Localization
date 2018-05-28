using Microsoft.EntityFrameworkCore.Migrations;

namespace Alto.Web.Migrations
{
    public partial class AddParentCategoryToToysAndFunctionMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update categories set parentcategoryid = 83 where id in (802, 807, 811, 813, 818, 821)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update categories set parentcategoryid = null where id in (802, 807, 811, 813, 818, 821)");
        }
    }
}
