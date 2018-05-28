using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Yooocan.Dal;

namespace Yooocan.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160703075713_intuseridmig")]
    partial class intuseridmig
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

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("IntId");

                    b.Property<string>("LastName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("Location");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("SecurityStamp");

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

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentCategoryId");

                    b.Property<string>("PictureUrl");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
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

                    b.Property<string>("FollowedUserId");

                    b.Property<string>("FollowerUserId");

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

                    b.Property<string>("UserId");

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

                    b.Property<bool>("IsPublished");

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

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductReviews");
                });

            modelBuilder.Entity("Yooocan.Entities.Story", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FeedImageUrl");

                    b.Property<string>("HeaderImageUrl");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("SearchImageUrl");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.Property<string>("YouTubeId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<int>("StoryId");

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

                    b.Property<string>("UserId");

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

                    b.ToTable("StoryImage");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryLimitation", b =>
                {
                    b.Property<int>("LimitationId");

                    b.Property<int>("StoryId");

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

                    b.ToTable("StoryParagraph");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryProduct", b =>
                {
                    b.Property<int>("StoryId");

                    b.Property<int>("ProductId");

                    b.HasKey("StoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryProduct");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTag", b =>
                {
                    b.Property<int>("StoryId");

                    b.Property<int>("TagId");

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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                        .HasForeignKey("FollowedUserId");

                    b.HasOne("Yooocan.Entities.ApplicationUser", "FollowerUser")
                        .WithMany("Follows")
                        .HasForeignKey("FollowerUserId");
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
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.Product", b =>
                {
                    b.HasOne("Yooocan.Entities.Vendor", "Vendor")
                        .WithMany("Products")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ProductCategory", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ProductImage", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ProductLimitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("ProductLimitations")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.ProductReview", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("ProductReviews")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.Story", b =>
                {
                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("Stories")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryCategory", b =>
                {
                    b.HasOne("Yooocan.Entities.Category", "Category")
                        .WithMany("StoryCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryCategories")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryComment", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Comments")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.ApplicationUser", "User")
                        .WithMany("StoryComments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Yooocan.Entities.StoryImage", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Images")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryLimitation", b =>
                {
                    b.HasOne("Yooocan.Entities.Limitation", "Limitation")
                        .WithMany()
                        .HasForeignKey("LimitationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryLimitations")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryParagraph", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("Paragraphs")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryProduct", b =>
                {
                    b.HasOne("Yooocan.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryProducts")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTag", b =>
                {
                    b.HasOne("Yooocan.Entities.Story", "Story")
                        .WithMany("StoryTags")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Yooocan.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Yooocan.Entities.StoryTip", b =>
                {
                    b.HasOne("Yooocan.Entities.Story")
                        .WithMany("Tips")
                        .HasForeignKey("StoryId");
                });
        }
    }
}
