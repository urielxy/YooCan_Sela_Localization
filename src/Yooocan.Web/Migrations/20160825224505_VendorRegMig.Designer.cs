﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Dal;

namespace Yooocan.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160825224505_VendorRegMig")]
    partial class VendorRegMig
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Yooocan.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("AboutMe");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Country");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("LastName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<double?>("Latitude");

                    b.Property<string>("Location");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<double?>("Longitude");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("PostalCode");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("State");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Yooocan.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HeaderPictureUrl");

                    b.Property<bool>("IsActiveForFeed");

                    b.Property<bool>("IsActiveForShop");

                    b.Property<bool>("IsChoosableForProduct");

                    b.Property<bool>("IsChoosableForStory");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentCategoryId");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("ShopBackgroundColor")
                        .IsRequired()
                        .HasColumnType("CHAR(6)");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Yooocan.Entities.Feed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Location");

                    b.Property<int?>("ParentCategoryId");

                    b.Property<string>("PrimaryImageUrl");

                    b.Property<int>("SubCategoryId");

                    b.Property<string>("SubCategoryName");

                    b.Property<string>("Title");

                    b.Property<string>("UserImageUrl");

                    b.HasKey("Id");

                    b.ToTable("Feed");
                });

            modelBuilder.Entity("Yooocan.Entities.FileUpload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsUsed");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("FileUploads");
                });

            modelBuilder.Entity("Yooocan.Entities.FollowerFollowed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FollowedUserId")
                        .IsRequired();

                    b.Property<string>("FollowerUserId")
                        .IsRequired();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.HasIndex("FollowedUserId");

                    b.HasIndex("FollowerUserId");

                    b.ToTable("Followers");
                });

            modelBuilder.Entity("Yooocan.Entities.Limitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentLimitationId");

                    b.HasKey("Id");

                    b.HasIndex("ParentLimitationId");

                    b.ToTable("Limitations");
                });

            modelBuilder.Entity("Yooocan.Entities.NotificationLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsSuccess");

                    b.Property<string>("Method");

                    b.Property<string>("NotificationId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationLogs");
                });

            modelBuilder.Entity("Yooocan.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("About");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<decimal?>("ListPrice");

                    b.Property<string>("Name");

                    b.Property<decimal>("Price");

                    b.Property<string>("Specifications");

                    b.Property<string>("Url");

                    b.Property<int>("VendorId");

                    b.Property<string>("VideoUrl");

                    b.HasKey("Id");

                    b.HasIndex("VendorId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<int>("ProductId");

                    b.HasKey("CategoryId", "ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CdnUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPrimary");

                    b.Property<int>("Order");

                    b.Property<string>("OriginalUrl");

                    b.Property<int>("ProductId");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductLimitation", b =>
                {
                    b.Property<int>("LimitationId");

                    b.Property<int>("ProductId");

                    b.HasKey("LimitationId", "ProductId");

                    b.HasIndex("LimitationId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductLimitations");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ProductId");

                    b.Property<byte>("Rating");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductReviews");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AboutTheCompany");

                    b.Property<string>("AdditionalInformation");

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("ContactPresonName");

                    b.Property<string>("Country");

                    b.Property<string>("Email");

                    b.Property<string>("Facebook");

                    b.Property<string>("HeaderImageUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Instagram");

                    b.Property<bool>("IsChapter");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<double?>("Latitude");

                    b.Property<string>("LogoUrl");

                    b.Property<double?>("Longitude");

                    b.Property<string>("MobileNumber");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostalCode");

                    b.Property<string>("State");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("TollFreePhoneNumber");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("ServiceProviders");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OpenDays");

                    b.Property<decimal>("Price");

                    b.Property<int?>("ServiceProviderId");

                    b.Property<string>("Units");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderActivities");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CdnUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPrimaryImage");

                    b.Property<int>("Order");

                    b.Property<int>("ServiceProviderId");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderImages");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderVideo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPrimaryVideo");

                    b.Property<int>("Order");

                    b.Property<int>("ServiceProviderId");

                    b.Property<string>("YouTubeId");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderVideos");
                });

            modelBuilder.Entity("Yooocan.Entities.Story", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivityLocation");

                    b.Property<string>("GooglePlaceId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<double?>("Latitude");

                    b.Property<double?>("Longitude");

                    b.Property<string>("Title");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<string>("YouTubeId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<int>("StoryId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("CategoryId", "StoryId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryCategories");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("StoryId");

                    b.Property<string>("Text");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryComments");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CdnUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Order");

                    b.Property<int>("StoryId");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryImages");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryLimitation", b =>
                {
                    b.Property<int>("LimitationId");

                    b.Property<int>("StoryId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("LimitationId", "StoryId");

                    b.HasIndex("LimitationId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryLimitations");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryParagraph", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Order");

                    b.Property<int>("StoryId");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryParagraphs");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryProduct", b =>
                {
                    b.Property<int>("StoryId");

                    b.Property<int>("ProductId");

                    b.HasKey("StoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryProducts");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTag", b =>
                {
                    b.Property<int>("StoryId");

                    b.Property<int>("TagId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("StoryId", "TagId");

                    b.HasIndex("StoryId");

                    b.HasIndex("TagId");

                    b.ToTable("StoryTags");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Order");

                    b.Property<int?>("StoryId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryTips");
                });

            modelBuilder.Entity("Yooocan.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Yooocan.Entities.Vendor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("Yooocan.Entities.VendorRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int>("BusinessType");

                    b.Property<string>("BusinessTypeOther");

                    b.Property<int>("CommercialMode");

                    b.Property<string>("ContactPresonName");

                    b.Property<string>("Email");

                    b.Property<string>("Facebook");

                    b.Property<decimal?>("FreeShippingOver");

                    b.Property<string>("Instagram");

                    b.Property<bool>("InterestedInAds");

                    b.Property<bool>("InterestedInSponsorship");

                    b.Property<bool>("MegaEcommercePlatforms");

                    b.Property<string>("MobileNumber");

                    b.Property<int>("NumberOfProducts");

                    b.Property<bool>("OwnEcommerceWebsite");

                    b.Property<string>("PhoneNumber");

                    b.Property<int>("ShippingCost");

                    b.Property<bool>("SpecializedDealerEommerceWebsite");

                    b.Property<string>("TollFreePhoneNumber");

                    b.Property<int>("VendorId");

                    b.HasKey("Id");

                    b.HasIndex("VendorId");

                    b.ToTable("VendorRegistrations");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.Category", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.FollowerFollowed", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "FollowedUser")
                        .WithMany("Followers")
                        .HasForeignKey("FollowedUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.ApplicationUser", "FollowerUser")
                        .WithMany("Follows")
                        .HasForeignKey("FollowerUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.Limitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "ParentLimitation")
                        .WithMany()
                        .HasForeignKey("ParentLimitationId");
                });

            modelBuilder.Entity("Yooocan.Entities.NotificationLog", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("NotificationLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.Product", b =>
                {
                    b.HasOne("Yooocan.Entities.Vendor", "Vendor")
                        .WithMany("Products")
                        .HasForeignKey("VendorId");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductCategory", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductImage", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductLimitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId");

                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("ProductLimitations")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Yooocan.Entities.ProductReview", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("ProductReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderActivity", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Activities")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderImage", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Images")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderVideo", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Videos")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.Story", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("Stories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryCategory", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany("StoryCategories")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryCategories")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryComment", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Comments")
                        .HasForeignKey("StoryId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("StoryComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryImage", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Images")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryLimitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId");

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryLimitations")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryParagraph", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Paragraphs")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryProduct", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryProducts")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTag", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryTags")
                        .HasForeignKey("StoryId");

                    b.HasOne("Yooocan.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTip", b =>
                {
                    b.HasOne("Yooocan.Entities.Story")
                        .WithMany("Tips")
                        .HasForeignKey("StoryId");
                });

            modelBuilder.Entity("Yooocan.Entities.VendorRegistration", b =>
                {
                    b.HasOne("Yooocan.Entities.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId");
                });
        }
    }
}
