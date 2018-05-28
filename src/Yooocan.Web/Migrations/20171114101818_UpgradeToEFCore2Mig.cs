using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Yooocan.Web.Migrations
{
    public partial class UpgradeToEFCore2Mig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFollowers_Categories_CategoryId",
                table: "CategoryFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeaturedStories_Stories_StoryId",
                table: "FeaturedStories");

            migrationBuilder.DropForeignKey(
                name: "FK_LimitationFollowers_Limitations_LimitationId",
                table: "LimitationFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationRecipients_Notifications_NotificationId",
                table: "NotificationRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Products_ProductId",
                table: "ProductLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReferrals_Products_ProductId",
                table: "ProductReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Vendors_VendorId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderCategories_Categories_CategoryId",
                table: "ServiceProviderCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderFollowers_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderImages_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderLimitations_Limitations_LimitationId",
                table: "ServiceProviderLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderLimitations_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderReferrals_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderVideos_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryCategories_Categories_CategoryId",
                table: "StoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryCategories_Stories_StoryId",
                table: "StoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryComments_Stories_StoryId",
                table: "StoryComments");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLimitations_Limitations_LimitationId",
                table: "StoryLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLimitations_Stories_StoryId",
                table: "StoryLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraphs");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryServiceProviders_ServiceProviders_ServiceProviderId",
                table: "StoryServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryServiceProviders_Stories_StoryId",
                table: "StoryServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTips_Stories_StoryId",
                table: "StoryTips");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFollowers_Categories_CategoryId",
                table: "CategoryFollowers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeaturedStories_Stories_StoryId",
                table: "FeaturedStories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LimitationFollowers_Limitations_LimitationId",
                table: "LimitationFollowers",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationRecipients_Notifications_NotificationId",
                table: "NotificationRecipients",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Products_ProductId",
                table: "ProductLimitations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReferrals_Products_ProductId",
                table: "ProductReferrals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Vendors_VendorId",
                table: "Products",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderCategories_Categories_CategoryId",
                table: "ServiceProviderCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderCategories",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderFollowers_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderFollowers",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderImages_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderImages",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderLimitations_Limitations_LimitationId",
                table: "ServiceProviderLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderLimitations_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderLimitations",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderReferrals_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderReferrals",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderVideos_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderVideos",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryCategories_Categories_CategoryId",
                table: "StoryCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryCategories_Stories_StoryId",
                table: "StoryCategories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryComments_Stories_StoryId",
                table: "StoryComments",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImages",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLimitations_Limitations_LimitationId",
                table: "StoryLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLimitations_Stories_StoryId",
                table: "StoryLimitations",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraphs",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProducts",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryServiceProviders_ServiceProviders_ServiceProviderId",
                table: "StoryServiceProviders",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryServiceProviders_Stories_StoryId",
                table: "StoryServiceProviders",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTips_Stories_StoryId",
                table: "StoryTips",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFollowers_Categories_CategoryId",
                table: "CategoryFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeaturedStories_Stories_StoryId",
                table: "FeaturedStories");

            migrationBuilder.DropForeignKey(
                name: "FK_LimitationFollowers_Limitations_LimitationId",
                table: "LimitationFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationRecipients_Notifications_NotificationId",
                table: "NotificationRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLimitations_Products_ProductId",
                table: "ProductLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReferrals_Products_ProductId",
                table: "ProductReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Vendors_VendorId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderCategories_Categories_CategoryId",
                table: "ServiceProviderCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderFollowers_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderImages_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderLimitations_Limitations_LimitationId",
                table: "ServiceProviderLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderLimitations_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderReferrals_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviderVideos_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryCategories_Categories_CategoryId",
                table: "StoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryCategories_Stories_StoryId",
                table: "StoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryComments_Stories_StoryId",
                table: "StoryComments");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLimitations_Limitations_LimitationId",
                table: "StoryLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLimitations_Stories_StoryId",
                table: "StoryLimitations");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraphs");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryServiceProviders_ServiceProviders_ServiceProviderId",
                table: "StoryServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryServiceProviders_Stories_StoryId",
                table: "StoryServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTips_Stories_StoryId",
                table: "StoryTips");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFollowers_Categories_CategoryId",
                table: "CategoryFollowers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeaturedStories_Stories_StoryId",
                table: "FeaturedStories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LimitationFollowers_Limitations_LimitationId",
                table: "LimitationFollowers",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationRecipients_Notifications_NotificationId",
                table: "NotificationRecipients",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Limitations_LimitationId",
                table: "ProductLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLimitations_Products_ProductId",
                table: "ProductLimitations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReferrals_Products_ProductId",
                table: "ProductReferrals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Vendors_VendorId",
                table: "Products",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderCategories_Categories_CategoryId",
                table: "ServiceProviderCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderCategories",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderContactRequests_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderContactRequests",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderFollowers_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderFollowers",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderImages_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderImages",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderLimitations_Limitations_LimitationId",
                table: "ServiceProviderLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderLimitations_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderLimitations",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderReferrals_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderReferrals",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviderVideos_ServiceProviders_ServiceProviderId",
                table: "ServiceProviderVideos",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryCategories_Categories_CategoryId",
                table: "StoryCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryCategories_Stories_StoryId",
                table: "StoryCategories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryComments_Stories_StoryId",
                table: "StoryComments",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryImages_Stories_StoryId",
                table: "StoryImages",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLimitations_Limitations_LimitationId",
                table: "StoryLimitations",
                column: "LimitationId",
                principalTable: "Limitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLimitations_Stories_StoryId",
                table: "StoryLimitations",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryParagraphs_Stories_StoryId",
                table: "StoryParagraphs",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Products_ProductId",
                table: "StoryProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryProducts_Stories_StoryId",
                table: "StoryProducts",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryServiceProviders_ServiceProviders_ServiceProviderId",
                table: "StoryServiceProviders",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryServiceProviders_Stories_StoryId",
                table: "StoryServiceProviders",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTips_Stories_StoryId",
                table: "StoryTips",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
