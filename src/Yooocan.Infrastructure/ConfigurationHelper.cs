using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Amazon;
using Yooocan.Logic.Messaging;
using Yooocan.Logic.Recaptchas;
using Yooocan.Logic.Utils.Shared;
using Yooocan.Logic.Options;
using Yooocan.Infrastructure.DataProtection;
using Yooocan.Logic.Images;
using System.Net.Http;
using Yooocan.Logic.Products;
using Yooocan.Logic.Benefits;
using Yooocan.Logic.Categories;

namespace Yooocan.Infrastructure
{
    public class ConfigurationHelper
    {
        private const string EmailConfirmationTokenProviderName = "ConfirmEmail";
        public IConfigurationRoot Configuration { get; }
        private string CurrentEnvironmentName { get; }

        public ConfigurationHelper(string environmentName = null, string basePath = null)
        {
            if (environmentName == null)
            {
                environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? EnvironmentName.Development;
            }
            CurrentEnvironmentName = environmentName;
            IConfigurationBuilder builder = new ConfigurationBuilder();
            if(basePath != null)
                builder = builder.SetBasePath(basePath);
                       
            Configuration = builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public bool IsDevelopment()
        {
            return CurrentEnvironmentName == EnvironmentName.Development;
        }

        private bool IsProduction()
        {
            return CurrentEnvironmentName == EnvironmentName.Production;
        }

        public IServiceCollection ConfigureServices(IServiceCollection services = null)
        {
            if (services == null)
                services = new ServiceCollection();
            
            services.Configure<AzureStorageOptions>(Configuration.GetSection("AzureStorage"));
            var azureStorageConfig = Configuration.GetSection("AzureStorage").Get<AzureStorageOptions>();
            var azureStorageAccountName = azureStorageConfig.AccountName;
            var azureStorageAccountKey = azureStorageConfig.AccountKey;

            var azureStorageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={azureStorageAccountName};AccountKey={azureStorageAccountKey}";
            var cloudStorageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            
            if (!IsDevelopment())
            {
                services.AddDataProtection()
                    .PersistKeysToAzureBlobStorage(cloudStorageAccount, "/aspnet-data-protection-keys/keys.xml");
            }
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Yooocan.Web")).ConfigureWarnings(x => x.Log());

                if (IsDevelopment())
                    options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(365);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
                {
                    option.Password.RequireDigit = false;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequiredLength = 1;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<ApplicationUser>>(EmailConfirmationTokenProviderName);

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.AccessDeniedPath = "/Account/Login";
            });

            var cdnPath = azureStorageConfig.ImagesCdnPath;
            var storagePath = azureStorageConfig.StoragePath;

            var mapper = new AutoMapperInitializer(storagePath, cdnPath).InitAutoMapper();
            services.AddSingleton(mapper);            

            // Add application services.
            services.AddTransient<IEmailSender, SendGridSender>();

            services.AddTransient<ISmsSender, SendGridSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddMemoryCache();
            string searchServiceName = "yooocanlive";

            string apiKey = "30BFA78AC9DB59D9D8F1AEB855828034";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            services.AddSingleton(serviceClient);
            services.AddSingleton(cloudStorageAccount);

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"));
            // Flush redis DB, careful with that.
            //if (CurrentEnvironment.IsDevelopment())
            //{
            //    redis.GetEndPoints().Select(x => redis.GetServer(x)).ToList().ForEach(x => x.FlushDatabase());
            //}
            services.AddSingleton(redis);

            services.AddSingleton(new HtmlSanitizer());

            services.AddTransient(_ => redis.GetDatabase());
            services.AddTransient<RedisWrapper>();

            services.Configure<AmazonOptions>(Configuration.GetSection("Amazon"));
            services.AddTransient<AmazonLogic>();

            services.Configure<ChatOptions>(Configuration.GetSection("IFlyChat"));

            services.Configure<FacebookKeys>(Configuration.GetSection("Facebook"));
            services.Configure<GoogleOAuthKeys>(Configuration.GetSection("GoogleOAuth"));

            services.AddTransient<ImageUrlOptimizer>();
            services.AddTransient<AzureImageResizer>();

            //https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
            services.AddSingleton<HttpClient>();

            AddLogicDependencies(services);

            return services;
        }

        public void ConfigureLogger(IServiceProvider container)
        {
            var seqAddress = "http://40.112.215.139:5341";
            if (IsDevelopment())
            {
                seqAddress = "http://localhost:5341";
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            var configurations = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Seq(seqAddress, apiKey: "WBgn78cl0R9m6rvvciP")
                .Enrich.FromLogContext();

            if (!IsProduction())
            {
                configurations.Enrich.WithProperty("Environment", CurrentEnvironmentName);
            }

            Log.Logger = configurations.CreateLogger();
            var loggerFactory = container.GetService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }

        private void AddLogicDependencies(IServiceCollection services)
        {
            services.AddTransient<IRecaptchaApi, RecaptchaApi>();            
            services.AddSingleton(_ => new SharedUserTokenManager(Configuration.GetValue<string>("YoocanTokenKey")));

            services.AddTransient<ILimitationLogic, LimitationLogic>();
            services.AddTransient<ICategoriesLogic, CategoriesLogic>();
            services.AddTransient<IAltoCategoryLogic, AltoCategoryLogic>();
            services.AddTransient<IHomeLogic, HomeLogic>();
            services.AddTransient<IUserLogic, UserLogic>();
            services.AddTransient<IStoryLogic, StoryLogic>();
            services.AddTransient<SearchLogic>();
            services.AddTransient<IOldProductLogic, OldProductLogic>();
            services.AddTransient<IProductLogic, ProductLogic>();
            services.AddTransient<IBenefitLogic, BenefitLogic>();
            services.AddTransient<IBlobUploader, AzureUploader>();
            services.AddTransient<IShopLogic, ShopLogic>();
            services.AddTransient<IAdminLogic, AdminLogic>();
            services.AddTransient<IImageLogic, ImageLogic>();
            services.AddTransient<IEmailLogic, EmailLogic>();
            services.AddTransient<IPrivateMessageLogic, PrivateMessageLogic>();
            services.AddTransient<IServiceProviderLogic, ServiceProviderLogic>();
            services.AddTransient<INotificationLogic, NotificationLogic>();
        }
    }
}
