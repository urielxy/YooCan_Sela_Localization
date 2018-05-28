using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Alto.Dal;

namespace Alto.Web.Migrations
{
    [DbContext(typeof(AltoDbContext))]
    [Migration("20170115165245_AddFewCompanyRelatedStuffMig")]
    partial class AddFewCompanyRelatedStuffMig
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Alto.Domain.AltoUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

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

            modelBuilder.Entity("Alto.Domain.Benefit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<decimal?>("Discount");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<DateTime>("LastUpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Benefits");
                });

            modelBuilder.Entity("Alto.Domain.BenefitCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BenefitId");

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.HasKey("Id");

                    b.HasIndex("BenefitId");

                    b.HasIndex("CategoryId");

                    b.ToTable("BenefitCategories");
                });

            modelBuilder.Entity("Alto.Domain.BenefitImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BenefitId");

                    b.Property<string>("CdnUrl");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("Order");

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("BenefitId");

                    b.ToTable("BenefitImages");
                });

            modelBuilder.Entity("Alto.Domain.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<int>("CompanyId");

                    b.Property<string>("Country");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<DateTime>("LastUpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<float?>("Latitude");

                    b.Property<float?>("Longitude");

                    b.Property<string>("Name");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("Alto.Domain.BranchBenefit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BenefitId");

                    b.Property<int>("BranchId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.HasKey("Id");

                    b.HasIndex("BenefitId");

                    b.HasIndex("BranchId");

                    b.ToTable("BranchBenefits");
                });

            modelBuilder.Entity("Alto.Domain.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Alto.Domain.CategoryImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("CdnUrl")
                        .IsRequired();

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("Type");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryImages");
                });

            modelBuilder.Entity("Alto.Domain.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("About");

                    b.Property<int>("BusinessType");

                    b.Property<string>("BusinessTypeOther");

                    b.Property<string>("City");

                    b.Property<string>("CompanyCode");

                    b.Property<string>("Country");

                    b.Property<string>("CouponCode");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("FaxNumber");

                    b.Property<string>("GooglePlaceId");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("LastUpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<double?>("Latitude");

                    b.Property<string>("LocationText");

                    b.Property<double?>("Longitude");

                    b.Property<decimal?>("MembersDiscountRate");

                    b.Property<string>("Name");

                    b.Property<string>("OnBoardingContactPersonEmail");

                    b.Property<DateTime?>("OnBoardingDate");

                    b.Property<string>("PostalCode");

                    b.Property<int>("RateType");

                    b.Property<decimal?>("ReferralRate");

                    b.Property<string>("State");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("TelephoneNumber");

                    b.Property<string>("TollFreeNumber");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Alto.Domain.CompanyCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<int>("CompanyId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CompanyId");

                    b.ToTable("CompanyCategory");
                });

            modelBuilder.Entity("Alto.Domain.CompanyContactPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("ContactPersonEmail");

                    b.Property<string>("ContactPersonName");

                    b.Property<string>("ContactPersonPhoneNumber");

                    b.Property<string>("ContactPersonPosition");

                    b.Property<string>("ContactPersonSkype");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("CompanyContactPersons");
                });

            modelBuilder.Entity("Alto.Domain.CompanyImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CdnUrl");

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("CompanyImages");
                });

            modelBuilder.Entity("Alto.Domain.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Alto.Domain.UserLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime>("InsertDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<DateTime>("LastUpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "GetUtcDate()");

                    b.Property<float?>("Latitude");

                    b.Property<float?>("Longitude");

                    b.Property<string>("State");

                    b.Property<int>("UserId");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserLocations");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Alto.Domain.Benefit", b =>
                {
                    b.HasOne("Alto.Domain.Company", "Company")
                        .WithMany("Benefits")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Alto.Domain.BenefitCategory", b =>
                {
                    b.HasOne("Alto.Domain.Benefit", "Benefit")
                        .WithMany("Categories")
                        .HasForeignKey("BenefitId");

                    b.HasOne("Alto.Domain.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Alto.Domain.BenefitImage", b =>
                {
                    b.HasOne("Alto.Domain.Benefit")
                        .WithMany("Images")
                        .HasForeignKey("BenefitId");
                });

            modelBuilder.Entity("Alto.Domain.Branch", b =>
                {
                    b.HasOne("Alto.Domain.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Alto.Domain.BranchBenefit", b =>
                {
                    b.HasOne("Alto.Domain.Benefit", "Benefit")
                        .WithMany("BenefitBranches")
                        .HasForeignKey("BenefitId");

                    b.HasOne("Alto.Domain.Branch", "Branch")
                        .WithMany("BranchBenefits")
                        .HasForeignKey("BranchId");
                });

            modelBuilder.Entity("Alto.Domain.Category", b =>
                {
                    b.HasOne("Alto.Domain.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId");
                });

            modelBuilder.Entity("Alto.Domain.CategoryImage", b =>
                {
                    b.HasOne("Alto.Domain.Category", "Category")
                        .WithMany("Images")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Alto.Domain.CompanyCategory", b =>
                {
                    b.HasOne("Alto.Domain.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Alto.Domain.Company", "Company")
                        .WithMany("Categories")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Alto.Domain.CompanyContactPerson", b =>
                {
                    b.HasOne("Alto.Domain.Company", "Company")
                        .WithMany("ContactPersons")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Alto.Domain.CompanyImage", b =>
                {
                    b.HasOne("Alto.Domain.Company")
                        .WithMany("Images")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Alto.Domain.UserLocation", b =>
                {
                    b.HasOne("Alto.Domain.AltoUser", "User")
                        .WithOne("Location")
                        .HasForeignKey("Alto.Domain.UserLocation", "UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Alto.Domain.Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Alto.Domain.AltoUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Alto.Domain.AltoUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Alto.Domain.Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("Alto.Domain.AltoUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });
        }
    }
}
