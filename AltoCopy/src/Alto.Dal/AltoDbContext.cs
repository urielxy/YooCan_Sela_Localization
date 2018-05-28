using System;
using System.Linq;
using Alto.Domain;
using Alto.Domain.Orders;
using Alto.Domain.Benefits;
using Alto.Domain.Companies;
using Alto.Domain.Products;
using Alto.Domain.Referrals;
using Alto.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alto.Dal
{
    public class AltoDbContext : IdentityDbContext<AltoUser, Role, int>
    {
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductLimitation> ProductLimitations { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyCoupon> CompanyCoupons { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<BenefitImage> BenefitImages { get; set; }
        public DbSet<CompanyImage> CompanyImages { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<UserFutureService> UserFutureServices { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BenefitCategory> BenefitCategories { get; set; }
        public DbSet<CategoryImage> CategoryImages { get; set; }
        public DbSet<CompanyContactPerson> CompanyContactPersons { get; set; }
        public DbSet<PromotedBenefit> PromotedBenefits { get; set; }
        public DbSet<PromotedProduct> PromotedProducts { get; set; }
        public DbSet<Variation> Variations { get; set; }
        public DbSet<ProductVariationValue> ProductVariationValues { get; set; }
        public DbSet<ProductVariationCombination> ProductVariationCombinations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<CompanyShipping> CompanyShippings { get; set; }
        public DbSet<ProductShipping> ProductShippings { get; set; }
        public DbSet<RegistrationPromo> RegistrationPromos { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<Limitation> Limitations { get; set; }
        public DbSet<ProductReferral> ProductReferrals { get; set; }
        public DbSet<BenefitReferral> BenefitReferrals { get; set; }

        public DbSet<Domain.Imports.Product> ImportProducts { get; set; }
        public DbSet<Domain.Imports.ProductImage> ImportProductImages { get; set; }
        public DbSet<Domain.Imports.Vendor> ImportVendors { get; set; }
        public AltoDbContext(DbContextOptions<AltoDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Disable FK cascade delete 
            builder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()
                    .Where(f => f.DeleteBehavior != DeleteBehavior.Restrict))
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

            ConfigDefaultInsertDate(builder);
            ConfigIdentityTables(builder);
            ConfigBranch(builder);
            ConfigCategories(builder);
            ConfigUser(builder);
            ConfigCompany(builder);
            ConfigProduct(builder);
            ConfigBenefit(builder);
            ConfigImports(builder);
            ConfigOrders(builder);
            ConfigReferrals(builder);
        }

        private void ConfigReferrals(ModelBuilder builder)
        {
            builder.Entity<ProductReferral>(r =>
            {
                r.Property(x => x.Ip).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });

            builder.Entity<BenefitReferral>(r =>
            {
                r.Property(x => x.Ip).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });
        }

        private void ConfigImports(ModelBuilder builder)
        {
            builder.Entity<Domain.Imports.Product>(r =>
            {
                r.ToTable("Products", "Imports");
            });

            builder.Entity<Domain.Imports.Vendor>(r =>
            {
                r.ToTable("Vendors", "Imports");
            });

            builder.Entity<Domain.Imports.ProductImage>(r =>
            {
                r.ToTable("ProductImages", "Imports");
            });
        }

        private void ConfigOrders(ModelBuilder builder)
        {
            builder.Entity<Order>(r =>
            {
                // are not required, since we currently create the order before we ask for the address and the PayPal payment
                //r.Property(x => x.AddressLine1).IsRequired();
                //r.Property(x => x.Country).IsRequired();
                //r.Property(x => x.City).IsRequired();
                //r.Property(x => x.ZipCode).IsRequired();
                //r.Property(x => x.State).IsRequired();
                //r.Property(x => x.SaleId).IsRequired();
            });
        }

        private void ConfigProduct(ModelBuilder builder)
        {
            builder.Entity<Variation>(r =>
            {
                r.Property(x => x.Name).IsRequired();
            });

            builder.Entity<ProductVariationCombination>(r =>
            {
                r.Property(x => x.Combinations).IsRequired();
            });

            builder.Entity<ProductVariationValue>(r =>
            {
                r.Property(x => x.Value).IsRequired();
            });

            builder.Entity<ProductCategory>(r =>
            {
                r.HasIndex(x => new { x.CategoryId, x.ProductId }).IsUnique();
                r.HasOne(x => x.Product).WithMany(x => x.ProductCategories).HasForeignKey(x => x.ProductId);
            });

            builder.Entity<ProductLimitation>(r =>
            {
                r.HasIndex(x => new { x.LimitationId, x.ProductId }).IsUnique();
                r.HasOne(x => x.Product).WithMany(x => x.ProductLimitations).HasForeignKey(x => x.ProductId);
            });

            builder.Entity<ProductReview>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });

            builder.Entity<ProductImage>(r =>
            {
                r.Property(x => x.CdnUrl).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });
        }

        private void ConfigCategories(ModelBuilder builder)
        {
            builder.Entity<CategoryImage>(r =>
            {
                r.Property(x => x.Url).IsRequired();
                r.Property(x => x.CdnUrl).IsRequired();
            });
        }

        private void ConfigUser(ModelBuilder builder)
        {
            builder.Entity<UserLocation>()
                .HasOne(x => x.User)
                .WithOne(x => x.Location);

            builder.Entity<UserImage>(r =>
            {
                r.Property(x => x.Url).IsRequired();
                r.Property(x => x.CdnUrl).IsRequired();
            });
        }

        private void ConfigBranch(ModelBuilder builder)
        {
            builder.Entity<BranchBenefit>()
                .ToTable("BranchBenefits")
                .HasOne(bb => bb.Branch)
                .WithMany(b => b.BranchBenefits)
                .HasForeignKey(pt => pt.BranchId);

            builder.Entity<BranchBenefit>()
                .HasOne(bb => bb.Benefit)
                .WithMany(b => b.BenefitBranches)
                .HasForeignKey(bb => bb.BenefitId);
        }

        private void ConfigCompany(ModelBuilder builder)
        {
            builder.Entity<CompanyContactPerson>(r =>
            {
                r.Property(x => x.Name).IsRequired();
                r.Property(x => x.Email).IsRequired();
            });

            builder.Entity<Company>(r =>
            {
                r.HasMany(c => c.Coupons)
                    .WithOne(coupon => coupon.Company)
                    .OnDelete(DeleteBehavior.Cascade);

                r.HasMany(c => c.ShippingRules)
                    .WithOne(s => s.Company)
                    .OnDelete(DeleteBehavior.Cascade);

                r.HasMany(c => c.ContactPersons)
                    .WithOne(s => s.Company)
                    .OnDelete(DeleteBehavior.Cascade);

                r.HasMany(c => c.Categories)
                    .WithOne(s => s.Company)
                    .OnDelete(DeleteBehavior.Cascade);

                r.HasMany(c => c.Images)
                    .WithOne(s => s.Company)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CompanyCoupon>(r =>
            {
                r.Property(x => x.Code).IsRequired();
            });
        }

        private void ConfigBenefit(ModelBuilder builder)
        {
            builder.Entity<BenefitImage>(r =>
            {
                r.Property(x => x.CdnUrl).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });

            builder.Entity<Benefit>()
                .HasMany(b => b.Categories)
                .WithOne(bc => bc.Benefit)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigDefaultInsertDate(ModelBuilder builder)
        {
            builder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(x => x.Name == "InsertDate" && x.ClrType == typeof(DateTime))
                .ToList().ForEach(x =>
                {
                    x.SqlServer().DefaultValueSql = "GetUtcDate()";
                    x.ValueGenerated = ValueGenerated.OnAdd;
                });

            builder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(x => x.Name == "LastUpdateDate" && x.ClrType == typeof(DateTime))
                .ToList().ForEach(x =>
                {
                    x.SqlServer().DefaultValueSql = "GetUtcDate()";
                    x.ValueGenerated = ValueGenerated.OnAdd;
                });
        }

        private void ConfigIdentityTables(ModelBuilder builder)
        {
            builder.Entity<AltoUser>().ToTable("Users")
                .HasOne(x => x.ReferrerPromo)
                .WithMany()
                .HasForeignKey(x => x.ReferrerPromoId);

            builder.Entity<Role>().ToTable("Roles");

            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");

            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");

            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");

            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            builder.Entity<RegistrationPromo>(r =>
            {
                r.HasOne(x => x.ReferrerUser)
                    .WithMany()
                    .HasForeignKey(x => x.ReferrerUserId);

                r.Property(x => x.Amount).HasColumnType("decimal(18, 9)");
            });
        }
    }
}