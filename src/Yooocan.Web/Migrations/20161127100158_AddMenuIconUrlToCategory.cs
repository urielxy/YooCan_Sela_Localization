using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddMenuIconUrlToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MenuIconUrl",
                table: "Categories",
                nullable: true);

            migrationBuilder.Sql(@"update categories
set shopbackgroundcolor = '2196F3'
where name = 'AQUA'

update categories
set shopbackgroundcolor = '8BC34A'
where name = 'PLAY BALL'

update categories
set shopbackgroundcolor = 'f44336'
where name = 'CYCLE'

update categories
set shopbackgroundcolor = '009688'
where name = 'WCMX & SKATE'

update categories
set shopbackgroundcolor = '9c27b0'
where name = 'MOBILITY'

update categories
set shopbackgroundcolor = '9e9e9e'
where name = 'WINTER SPORTS'

update categories
set shopbackgroundcolor = 'ffc107'
where name = 'FITNESS'

update categories
set shopbackgroundcolor = 'e91e63'
where name = 'DANCE'

update categories
set shopbackgroundcolor = '795548'
where name = 'CAMP'

update categories
set shopbackgroundcolor = '673ab7'
where name = 'INNOVATE'

update categories
set shopbackgroundcolor = '34495e'
where name = 'ART & MUSIC'

update categories
set shopbackgroundcolor = 'ff5722'
where name = 'FAMILY LIFE'

update categories
set shopbackgroundcolor = 'ff9800'
where name = 'OTHERS'

update categories
set shopbackgroundcolor = '795548'
where name = 'HORSE RIDING'

update categories
set shopbackgroundcolor = 'f44336'
where name = 'MOTOR VEHICLES'

update categories
set shopbackgroundcolor = 'e91e63'
where name = 'STYLE'

update categories
set shopbackgroundcolor = '673ab7'
where name = 'LEARNING & EDUCATION'

update categories
set shopbackgroundcolor = '00bcd4'
where name = 'TRAVEL'");

            migrationBuilder.Sql(@"update categories set menuiconurl='/images/categories/menu/aqua.svg'  where [name] = 'AQUA';
update categories set menuiconurl='/images/categories/menu/playball.svg'  where [name] = 'PLAY BALL';
update categories set menuiconurl='/images/categories/menu/cycle.svg'  where [name] = 'CYCLE';
update categories set menuiconurl='/images/categories/menu/skate.svg'  where [name] = 'WCMX & SKATE';
update categories set menuiconurl='/images/categories/menu/mobility.svg'  where [name] = 'MOBILITY';
update categories set menuiconurl='/images/categories/menu/winter-sport.svg'  where [name] = 'WINTER SPORTS';
update categories set menuiconurl='/images/categories/menu/fitness.svg'  where [name] = 'FITNESS';
update categories set menuiconurl='/images/categories/menu/dance.svg'  where [name] = 'DANCE';
update categories set menuiconurl='/images/categories/menu/innovate.svg'  where [name] = 'INNOVATE';
update categories set menuiconurl='/images/categories/menu/music-art.svg'  where [name] = 'ART & MUSIC';
update categories set menuiconurl='/images/categories/menu/family-life.svg'  where [name] = 'FAMILY LIFE';
update categories set menuiconurl='/images/categories/menu/more.svg'  where [name] = 'OTHERS';
update categories set menuiconurl='/images/categories/menu/horse-riding.svg'  where [name] = 'HORSE RIDING';
update categories set menuiconurl='/images/categories/menu/motor.svg'  where [name] = 'MOTOR VEHICLES';
update categories set menuiconurl='/images/categories/menu/education.svg'  where [name] = 'LEARNING & EDUCATION';
update categories set menuiconurl='/images/categories/menu/camping.svg'  where [name] = 'CAMP';
update categories set menuiconurl='/images/categories/menu/travel.svg'  where [name] = 'TRAVEL';
update categories set menuiconurl='/images/categories/menu/style.svg'  where [name] = 'STYLE';");

            migrationBuilder.Sql(@"update categories
set RoundIcon = Replace(headerpictureUrl, '/header/', '/icons/')
where isactiveforfeed = 1
and parentCategoryId  is null");
            migrationBuilder.Sql(@"update stories
set LikesCount =  (ABS(CHECKSUM(NewId())) % 11) + 5
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuIconUrl",
                table: "Categories");
        }
    }
}
