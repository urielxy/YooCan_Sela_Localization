using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Yooocan.Entities;
using Yooocan.Entities.Referrals;
using Yooocan.Entities.ServiceProviders;
using Microsoft.AspNetCore.Identity;
using Yooocan.Entities.Blog;
using Yooocan.Entities.Benefits;
using Yooocan.Entities.Companies;
using Yooocan.Entities.Products;

namespace Yooocan.Dal
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<StoryImage> StoryImages { get; set; }
        public DbSet<StoryProduct> StoryProducts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryLike> StoryLikes { get; set; }
        public DbSet<CategoryFollower> CategoryFollowers { get; set; }
        public DbSet<LimitationFollower> LimitationFollowers { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<PromotedProduct> PromotedProducts { get; set; }
        public DbSet<StoryComment> StoryComments { get; set; }
        public DbSet<Limitation> Limitations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<ProductLimitation> ProductLimitations { get; set; }
        public DbSet<StoryLimitation> StoryLimitations { get; set; }
        public DbSet<StoryCategory> StoryCategories { get; set; }
        public DbSet<StoryServiceProvider> StoryServiceProviders { get; set; }
        public DbSet<FollowerFollowed> Followers { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
        public DbSet<Feed> Feed { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceProviderCategory> ServiceProviderCategories { get; set; }
        public DbSet<ServiceProviderLimitation> ServiceProviderLimitations { get; set; }
        public DbSet<ServiceProviderContactRequest> ServiceProviderContactRequests { get; set; }
        public DbSet<ServiceProviderFollower> ServiceProviderFollowers { get; set; }
        public DbSet<VendorRegistration> VendorRegistrations { get; set; }
        public DbSet<PendingClaim> PendingClaims { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<ReadHistory> ReadHistories { get; set; }
        public DbSet<StoryParagraph> StoryParagraphs { get; set; }
        public DbSet<StoryTip> StoryTips { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<FeaturedStory> FeaturedStories { get; set; }
        public DbSet<ProductReferral> ProductReferrals { get; set; }
        public DbSet<ServiceProviderReferral> ServiceProviderReferrals { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }

        #region Alto Entities
        public DbSet<BenefitReferral> BenefitReferrals { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<BenefitCategory> BenefitCategories { get; set; }
        public DbSet<BenefitImage> BenefitImages { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyCoupon> CompanyCoupons { get; set; }
        public DbSet<CompanyImage> CompanyImages { get; set; }
        public DbSet<CompanyShipping> CompanyShippings { get; set; }
        public DbSet<CompanyContactPerson> CompanyContactPersons { get; set; }
        public DbSet<AltoCategory> AltoCategories { get; set; }
        public DbSet<AltoCategoryImage> AltoCategoryImages { get; set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Disable FK cascade delete 
            builder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()
                    .Where(f => f.DeleteBehavior == DeleteBehavior.Cascade))
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

            ConfigDefaultInsertDate(builder);
            ConfigIdentityTables(builder);

            ConfigUser(builder);
            ConfigPrivateMessages(builder);
            ConfigProduct(builder);

            ConfigCategory(builder);
            ConfigLimitation(builder);
            ConfigStory(builder);
            ConfigFollowers(builder);
            ConfigNotifications(builder);
            ConfigServiceProviders(builder);
            ConfigReferrals(builder);
            ConfigBlog(builder);
            ConfigCompany(builder);
            ConfigBenefit(builder);
        }

        private void ConfigCompany(ModelBuilder builder)
        {
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
        }

        private void ConfigBenefit(ModelBuilder builder)
        {
            builder.Entity<Benefit>()
                .HasMany(b => b.Categories)
                .WithOne(bc => bc.Benefit)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigBlog(ModelBuilder builder)
        {
            builder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<PostImage>().HasQueryFilter(p => !p.IsDeleted);
        }

        private void ConfigReferrals(ModelBuilder builder)
        {
            builder.Entity<ProductReferral>(r =>
            {
                r.Property(x => x.Ip).IsRequired();
                r.Property(x => x.UserAgent).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });

            builder.Entity<ServiceProviderReferral>(r =>
            {
                r.Property(x => x.Ip).IsRequired();
                r.Property(x => x.UserAgent).IsRequired();
                r.Property(x => x.Url).IsRequired();
            });
        }

        private void ConfigServiceProviders(ModelBuilder builder)
        {
            builder.Entity<ServiceProviderImage>().ToTable("ServiceProviderImages");
            builder.Entity<ServiceProviderVideo>().ToTable("ServiceProviderVideos");

            builder.Entity<ServiceProviderCategory>(r =>
            {
                r.HasKey(x => new { x.CategoryId, x.ServiceProviderId });
                r.HasOne(x => x.ServiceProvider).WithMany(x => x.ServiceProviderCategories).HasForeignKey(x => x.ServiceProviderId);
                r.HasIndex(x => x.CategoryId);
            });

            builder.Entity<ServiceProviderLimitation>(r =>
            {
                r.HasKey(x => new { x.LimitationId, x.ServiceProviderId });
                r.HasOne(x => x.ServiceProvider).WithMany(x => x.ServiceProviderLimitations).HasForeignKey(x => x.ServiceProviderId);
                r.HasIndex(x => x.LimitationId);
            });


            builder.Entity<ServiceProviderActivity>(r =>
            {
                r.ToTable("ServiceProviderActivities");
                r.Property(x => x.Name).IsRequired();
            });

            builder.Entity<ServiceProviderFollower>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });
        }

        private void ConfigNotifications(ModelBuilder builder)
        {
            builder.Entity<NotificationLog>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });

            builder.Entity<Notification>(r =>
            {
                r.HasMany(x => x.Recipients);
            });

            builder.Entity<NotificationRecipient>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });
        }

        private void ConfigFollowers(ModelBuilder builder)
        {
            builder.Entity<FollowerFollowed>(r =>
            {
                r.ToTable("Followers");
                r.HasOne(x => x.FollowedUser).WithMany(x => x.Followers).HasForeignKey(x => x.FollowedUserId).IsRequired();
                r.HasOne(x => x.FollowerUser).WithMany(x => x.Follows).HasForeignKey(x => x.FollowerUserId).IsRequired();
            });
        }

        private void ConfigStory(ModelBuilder builder)
        {
            builder.Entity<StoryProduct>(r =>
            {
                r.HasKey(x => new { x.StoryId, x.ProductId });
                r.HasOne(x => x.Story).WithMany(x => x.StoryProducts).HasForeignKey(x => x.StoryId);
                r.HasIndex(x => x.ProductId);
                r.HasIndex(x => x.StoryId);
            });

            builder.Entity<Story>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
                r.HasIndex(x => new { x.Id, x.PublishDate, x.IsPublished });
            });

            builder.Entity<StoryComment>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });
        }

        private void ConfigCategory(ModelBuilder builder)
        {
            builder.Entity<Category>(r =>
            {
                r.HasMany(x => x.SubCategories).WithOne(x => x.ParentCategory).HasForeignKey(x => x.ParentCategoryId);
                r.Property(x => x.ShopBackgroundColor).HasColumnType("CHAR(6)").IsRequired();
            });

            builder.Entity<ProductCategory>(r =>
            {
                r.HasKey(x => x.Id);
                r.HasIndex(x => new { x.CategoryId, x.ProductId }).IsUnique();
                r.HasOne(x => x.Product).WithMany(x => x.ProductCategories).HasForeignKey(x => x.ProductId);
            });

            builder.Entity<StoryCategory>(r =>
            {
                r.HasKey(x => new { x.CategoryId, x.StoryId });
                r.HasOne(x => x.Category).WithMany(x => x.StoryCategories).HasForeignKey(x => x.CategoryId);
                r.HasIndex(x => x.CategoryId);
            });

            builder.Entity<CategoryFollower>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
                r.HasIndex(x => new {x.UserId, x.CategoryId, x.DeleteDate});
                r.HasIndex(x => x.UserId);
            });
        }

        private void ConfigLimitation(ModelBuilder builder)
        {
            builder.Entity<ProductLimitation>(r =>
            {
                r.HasKey(x => new { x.LimitationId, x.ProductId });
                r.HasOne(x => x.Product).WithMany(x => x.ProductLimitations).HasForeignKey(x => x.ProductId);
                r.HasIndex(x => x.LimitationId);
            });

            builder.Entity<StoryLimitation>(r =>
            {
                r.HasKey(x => new { x.LimitationId, x.StoryId });
                r.HasOne(x => x.Story).WithMany(x => x.StoryLimitations).HasForeignKey(x => x.StoryId);
                r.HasIndex(x => x.LimitationId);
            });

            builder.Entity<LimitationFollower>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
                r.HasIndex(x => new {x.UserId, x.LimitationId, x.DeleteDate});
                r.HasIndex(x => x.LimitationId);
                r.HasIndex(x => x.UserId);
            });
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

        private void ConfigUser(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(r =>
            {
                r.Property(x => x.FirstName).HasMaxLength(100);
                r.Property(x => x.LastName).HasMaxLength(100);
                r.HasMany(x => x.Claims).WithOne().HasForeignKey(x => x.UserId);
            });

            builder.Entity<PendingClaim>(r =>
            {
                r.Property(x => x.Email).HasMaxLength(254);
                r.Property(x => x.CreatedById).IsRequired();
            });

            builder.Entity<ReadHistory>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
                r.HasIndex(x => new { x.UserId, x.InsertDate, x.CategoryId, x.StoryId });
                r.HasIndex(x => x.UserId);
            });

            builder.Entity<NewsletterSubscriber>(r =>
            {
                r.Property(x => x.Email).IsRequired().HasMaxLength(254);
                r.HasIndex(x => x.Email).IsUnique();
            });
        }

        private void ConfigProduct(ModelBuilder builder)
        {
            builder.Entity<Product>(r =>
            {
                r.HasOne(x => x.Company).WithMany(x => x.Products);
                r.HasIndex(x => x.AltoId);
                r.HasQueryFilter(p => !p.IsDeleted);
            });

            builder.Entity<ProductImage>(r =>
            {
                r.HasQueryFilter(p => !p.IsDeleted);
            });

            builder.Entity<ProductReview>(r =>
            {
                r.Property(x => x.UserId).IsRequired();
            });
        }

        private void ConfigIdentityTables(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("Users");

            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");

            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }

        private void ConfigPrivateMessages(ModelBuilder builder)
        {
            builder.Entity<PrivateMessage>(r =>
            {
                r.Property(x => x.Title).HasMaxLength(254);
                r.Property(x => x.FromUserId).IsRequired();
                r.Property(x => x.ToUserId).IsRequired();
            });
        }
    }
}
