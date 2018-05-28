using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yooocan.Web.Migrations
{
    public partial class FeedViewChangeMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER VIEW [dbo].[Feed]
AS 
SELECT S.Id, 
	   S.Title, 
	   ISNULL(SI.CdnUrl, SI.Url) AS PrimaryImageUrl, 
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
		ON  InnerStory.Id = SC.StoryId
	  WHERE InnerStory.IsPublished = 1
	  AND   SC.IsPrimary = 1
    ) T
INNER JOIN Categories C
	ON C.Id = T.CategoryId
INNER JOIN Stories S
	ON S.Id = T.StoryId
INNER JOIN Users U
	ON U.Id = S.UserId
LEFT JOIN StoryImages SI
	ON SI.StoryId = S.Id AND SI.IsDeleted = 0 AND SI.[Order] = 0
WHERE t.rowid <= 3;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER VIEW [dbo].[Feed]
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
		ON  InnerStory.Id = SC.StoryId
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
WHERE t.rowid <= 3;
");
        }
    }
}
