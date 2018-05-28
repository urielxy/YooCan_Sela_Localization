using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Dal;

namespace Yooocan.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161213175648_RemoveUselessColumnInContactSPMig")]
    partial class RemoveUselessColumnInContactSPMig
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

                    b.Property<bool>("CustomizedFeedDone");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("HeaderImageUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("InstagramUserName");

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

                    b.Property<string>("Description");

                    b.Property<int>("FollowersCount");

                    b.Property<string>("HeaderPictureUrl");

                    b.Property<bool>("IsActiveForFeed");

                    b.Property<bool>("IsActiveForShop");

                    b.Property<bool>("IsChoosableForProduct");

                    b.Property<bool>("IsChoosableForStory");

                    b.Property<string>("MenuIconUrl");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentCategoryId");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("RoundIcon");

                    b.Property<string>("ShopBackgroundColor")
                        .IsRequired()
                        .HasColumnType("CHAR(6)");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Yooocan.Entities.CategoryFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "CategoryId", "DeleteDate");

                    b.ToTable("CategoryFollowers");
                });

            modelBuilder.Entity("Yooocan.Entities.FeaturedStory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FeaturedType");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("StoryId");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("FeaturedStories");
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

            modelBuilder.Entity("Yooocan.Entities.LimitationFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("LimitationId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("LimitationId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "LimitationId", "DeleteDate");

                    b.ToTable("LimitationFollowers");
                });

            modelBuilder.Entity("Yooocan.Entities.NewsletterSubscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 254);

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("IpAddress");

                    b.Property<bool>("IsVerified");

                    b.Property<bool>("Unsubscribed");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("NewsletterSubscribers");
                });

            modelBuilder.Entity("Yooocan.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Link");

                    b.Property<int?>("ObjectId");

                    b.Property<int?>("ParentId");

                    b.Property<int?>("ParentType");

                    b.Property<string>("SourceUserId");

                    b.Property<string>("Text");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("SourceUserId");

                    b.ToTable("Notifications");
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

            modelBuilder.Entity("Yooocan.Entities.NotificationRecipient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("NotificationId");

                    b.Property<DateTime?>("ReadDate");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationRecipients");
                });

            modelBuilder.Entity("Yooocan.Entities.PendingClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 254);

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<bool>("WasAssigned");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("PendingClaims");
                });

            modelBuilder.Entity("Yooocan.Entities.PrivateMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("FromUserId")
                        .IsRequired();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeletedByRecipient");

                    b.Property<bool>("IsDeletedBySender");

                    b.Property<DateTime?>("ReadDate");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 254);

                    b.Property<string>("ToUserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("PrivateMessages");
                });

            modelBuilder.Entity("Yooocan.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("About");

                    b.Property<string>("Brand");

                    b.Property<string>("Colors");

                    b.Property<float?>("Depth");

                    b.Property<float?>("Height");

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

                    b.Property<string>("Upc");

                    b.Property<string>("Url");

                    b.Property<int>("VendorId");

                    b.Property<string>("VideoUrl");

                    b.Property<string>("WarrentyUrl");

                    b.Property<float?>("Weight");

                    b.Property<float?>("Width");

                    b.Property<string>("YouTubeId");

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

                    b.Property<int>("Order");

                    b.Property<string>("OriginalUrl");

                    b.Property<int>("ProductId");

                    b.Property<int>("Type");

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

            modelBuilder.Entity("Yooocan.Entities.ReadHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int?>("StoryId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "InsertDate", "CategoryId", "StoryId");

                    b.ToTable("ReadHistories");
                });

            modelBuilder.Entity("Yooocan.Entities.Referrals.ProductReferral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Ip")
                        .IsRequired();

                    b.Property<int>("ProductId");

                    b.Property<string>("Referrer");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.Property<string>("UserAgent")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductReferrals");
                });

            modelBuilder.Entity("Yooocan.Entities.Referrals.ServiceProviderReferral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Ip")
                        .IsRequired();

                    b.Property<string>("Referrer");

                    b.Property<int>("ServiceProviderId");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.Property<string>("UserAgent")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.HasIndex("UserId");

                    b.ToTable("ServiceProviderReferrals");
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

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OpenDays");

                    b.Property<int>("Order");

                    b.Property<decimal>("Price");

                    b.Property<int?>("ServiceProviderId");

                    b.Property<string>("Units");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderActivities");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<int>("ServiceProviderId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("CategoryId", "ServiceProviderId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderCategories");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderContactRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Email");

                    b.Property<string>("Message");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<int>("ServiceProviderId");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderContactRequests");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("ServiceProviderId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.HasIndex("UserId");

                    b.ToTable("ServiceProviderFollowers");
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

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderLimitation", b =>
                {
                    b.Property<int>("LimitationId");

                    b.Property<int>("ServiceProviderId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("LimitationId", "ServiceProviderId");

                    b.HasIndex("LimitationId");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("ServiceProviderLimitations");
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

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.StoryServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Order");

                    b.Property<int>("ServiceProviderId");

                    b.Property<int>("StoryId");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryServiceProviders");
                });

            modelBuilder.Entity("Yooocan.Entities.Story", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivityLocation");

                    b.Property<bool>("CanNotBeFeaturedOnHomePage");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("GooglePlaceId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsProductsReviewed");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<double?>("Latitude");

                    b.Property<int>("LikesCount");

                    b.Property<double?>("Longitude");

                    b.Property<string>("PostalCode");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<string>("Quote");

                    b.Property<string>("State");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("Title");

                    b.Property<string>("UsedProducts");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<int>("ViewsCount");

                    b.Property<string>("YouTubeId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Id", "PublishDate", "IsPublished");

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

                    b.Property<bool>("IsPrimary");

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

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryImages");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryLike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("StoryId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryLikes");
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

                    b.Property<bool>("IsUsedInStory");

                    b.Property<int>("Order");

                    b.HasKey("StoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryProducts");
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

                    b.Property<int>("StoryId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryTips");
                });

            modelBuilder.Entity("Yooocan.Entities.Vendor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("About");

                    b.Property<int>("BusinessType");

                    b.Property<string>("BusinessTypeOther");

                    b.Property<string>("City");

                    b.Property<int?>("CommercialTerms");

                    b.Property<string>("CommercialTermsOther");

                    b.Property<decimal?>("CommercialTermsRate");

                    b.Property<string>("CompanyCode");

                    b.Property<string>("ContactPersonEmail");

                    b.Property<string>("ContactPersonName");

                    b.Property<string>("ContactPersonPhoneNumber");

                    b.Property<string>("ContactPersonPosition");

                    b.Property<string>("ContactPersonSkype");

                    b.Property<string>("Country");

                    b.Property<string>("Facebook");

                    b.Property<string>("FaxNumber");

                    b.Property<string>("GooglePlaceId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Instagram");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastUpdateDate");

                    b.Property<double?>("Latitude");

                    b.Property<string>("LocationText");

                    b.Property<string>("LogoUrl");

                    b.Property<double?>("Longitude");

                    b.Property<string>("Name");

                    b.Property<string>("OnBoardingContactPersonEmail");

                    b.Property<DateTime?>("OnBoardingDate");

                    b.Property<string>("PostalCode");

                    b.Property<string>("State");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("TelephoneNumber");

                    b.Property<string>("TollFreeNumber");

                    b.Property<string>("Twitter");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("Yooocan.Entities.VendorRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessType");

                    b.Property<string>("BusinessTypeOther");

                    b.Property<string>("ContactPresonName");

                    b.Property<string>("ContactPresonRole");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("WasHandled");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

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

            modelBuilder.Entity("Yooocan.Entities.CategoryFollower", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("Categories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.FeaturedStory", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany()
                        .HasForeignKey("StoryId");
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

            modelBuilder.Entity("Yooocan.Entities.LimitationFollower", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("Limitations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.Notification", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "SourceUser")
                        .WithMany()
                        .HasForeignKey("SourceUserId");
                });

            modelBuilder.Entity("Yooocan.Entities.NotificationLog", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("NotificationLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.NotificationRecipient", b =>
                {
                    b.HasOne("Yooocan.Entities.Notification", "Notification")
                        .WithMany("Recipients")
                        .HasForeignKey("NotificationId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.PendingClaim", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.PrivateMessage", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.ApplicationUser", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserId")
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

            modelBuilder.Entity("Yooocan.Entities.ReadHistory", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.Referrals.ProductReferral", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.Referrals.ServiceProviderReferral", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "Product")
                        .WithMany()
                        .HasForeignKey("ServiceProviderId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderActivity", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Activities")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderCategory", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany("ServiceProviderCategories")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("ServiceProviderCategories")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderContactRequest", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("ServiceProviderContactRequests")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderFollower", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany()
                        .HasForeignKey("ServiceProviderId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderImage", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Images")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderLimitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId");

                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("ServiceProviderLimitations")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.ServiceProviderVideo", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany("Videos")
                        .HasForeignKey("ServiceProviderId");
                });

            modelBuilder.Entity("Yooocan.Entities.ServiceProviders.StoryServiceProvider", b =>
                {
                    b.HasOne("Yooocan.Entities.ServiceProviders.ServiceProvider", "ServiceProvider")
                        .WithMany()
                        .HasForeignKey("ServiceProviderId");

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryServiceProviders")
                        .HasForeignKey("StoryId");
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

            modelBuilder.Entity("Yooocan.Entities.StoryLike", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany()
                        .HasForeignKey("StoryId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
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

            modelBuilder.Entity("Yooocan.Entities.StoryTip", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Tips")
                        .HasForeignKey("StoryId");
                });
        }
    }
}
