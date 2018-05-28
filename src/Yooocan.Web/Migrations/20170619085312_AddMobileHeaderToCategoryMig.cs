using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddMobileHeaderToCategoryMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobileHeaderPictureUrl",
                table: "Categories",
                nullable: true);

            migrationBuilder.Sql(@"update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/aqua.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/AQUA2.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/artmusic.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/LEARN, ART & MUSIC.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/camp.jpg' where headerpictureurl = '/images/categories/header/camp.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/cycling.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/CYCLE.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/dance.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/DANCE.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/familylife.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/family life.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/fitness.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/FITNESS.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/horseriding.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/Horse Riding.jpg';
update [Categories] set mobileheaderpictureurl = 'https://yooocanlive.azureedge.net/categories/header/mobile/innovate.jpg' where headerpictureurl = 'https://yooocanlive.azureedge.net/categories/header/INNOVATE.jpg';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileHeaderPictureUrl",
                table: "Categories");
        }
    }
}
