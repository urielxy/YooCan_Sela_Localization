using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class AddLocationDataToUserMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Users",
                nullable: true);

            migrationBuilder.Sql(@"IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS
WHERE TABLE_NAME = 'Feed')
DROP VIEW Feed");

            migrationBuilder.Sql(@"CREATE VIEW Feed
AS 
SELECT S.Id, 
	   S.Title, 
	   SI.Url AS PrimaryImageUrl, 
	   U.FirstName, 
	   U.LastName, 	   
	   CASE
			WHEN ISNULL(U.City, '') = '' OR ISNULL(U.Country, '') = '' 
			THEN U.Location
			WHEN U.Country = 'United States'
			THEN CONCAT(U.City, ' ', U.State,', ', 'USA')
			ELSE CONCAT(U.City, ', ', U.Country)				
	   END as Location,
	   U.PictureUrl AS UserImageUrl, 
	   C.Id AS SubCategoryId, 
	   C.Name AS SubCategoryName, 
	   C.ParentCategoryId
FROM (SELECT 
		SC.StoryId,
		SC.CategoryId,
		rowid = ROW_NUMBER() OVER (PARTITION BY SC.CategoryId ORDER BY InnerStory.InsertDate desc)
      FROM StoryCategories SC
	  INNER JOIN Stories InnerStory
		ON InnerStory.Id = SC.StoryId
	  WHERE InnerStory.IsPublished = 1 
    ) T
INNER JOIN Categories C
	ON C.Id = T.CategoryId
INNER JOIN Stories S
	ON S.Id = T.StoryId
INNER JOIN Users U
	ON U.Id = S.UserId
LEFT JOIN StoryImages SI
	ON SI.StoryId = S.Id AND SI.IsDeleted = 0 AND SI.[Order] = 0
WHERE t.rowid <= 3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Users");

            migrationBuilder.Sql(@"IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS
WHERE TABLE_NAME = 'Feed')
DROP VIEW Feed");
            migrationBuilder.Sql(@"CREATE VIEW Feed
AS 
SELECT S.Id, 
	   S.Title, 
	   SI.Url AS PrimaryImageUrl, 
	   U.FirstName, 
	   U.LastName, 	   
	   U.Location AS Location,
	   U.PictureUrl AS UserImageUrl, 
	   C.Id AS SubCategoryId, 
	   C.Name AS SubCategoryName, 
	   C.ParentCategoryId
FROM (SELECT 
		SC.StoryId,
		SC.CategoryId,
		rowid = ROW_NUMBER() OVER (PARTITION BY SC.CategoryId ORDER BY InnerStory.InsertDate desc)
      FROM StoryCategories SC
	  INNER JOIN Stories InnerStory
		ON InnerStory.Id = SC.StoryId
	  WHERE InnerStory.IsPublished = 1 
    ) T
INNER JOIN Categories C
	ON C.Id = T.CategoryId
INNER JOIN Stories S
	ON S.Id = T.StoryId
INNER JOIN Users U
	ON U.Id = S.UserId
LEFT JOIN StoryImages SI
	ON SI.StoryId = S.Id AND SI.IsDeleted = 0 AND SI.[Order] = 0
WHERE t.rowid <= 3");

            migrationBuilder.AddColumn<int>(
                name: "IntId",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }
    }
}
