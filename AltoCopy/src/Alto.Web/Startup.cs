using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Users;
using Alto.Logic;
using Alto.Logic.Categories;
using Alto.Logic.External;
using Alto.Logic.Messaging;
using Alto.Logic.PayPal;
using Alto.Logic.Search;
using Alto.Logic.Upload;
using Alto.Logic.Utils.Shared;
using Alto.Web.AutoMapper;
using Alto.Web.Middlewares;
using Alto.Web.Utils;
using Alto.Web.Utils.Yoocan;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

namespace Alto.Web
{
    public class Startup
    {
        private const string EmailConfirmationTokenProviderName = "ConfirmEmail";
        public IHostingEnvironment CurrentEnvironment { get; set; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            var seqAddress = "http://40.112.215.139:5341";
            CurrentEnvironment = env;
            if (env.IsDevelopment())
            {
                seqAddress = "http://localhost:5341";
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            var configurations = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Seq(seqAddress, apiKey: "WBgn78cl0R9m6rvvciP")
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Alto");

            if (!env.IsProduction())
            {
                configurations.Enrich.WithProperty("Environment", env.EnvironmentName);
            }

            Log.Logger = configurations.CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var azureStorageConfig = Configuration.GetSection("AzureStorage");
            var azureStorageAccountName = azureStorageConfig["AccountName"];
            var azureStorageAccountKey = azureStorageConfig["AccountKey"];
            var azureStorageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={azureStorageAccountName};AccountKey={azureStorageAccountKey}";
            var cloudStorageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);

            if (CurrentEnvironment.IsProduction() || CurrentEnvironment.IsStaging())
            {
                services.AddDataProtection()
                    .PersistKeysToAzureBlobStorage(cloudStorageAccount, "/aspnet-data-protection-keys/keys.xml");
            }

            var azureSearchOptions = new AzureSearchOptions();
            Configuration.GetSection("AzureSearch").Bind(azureSearchOptions);
            var serviceClient = new SearchServiceClient(azureSearchOptions.ServiceName, new SearchCredentials(azureSearchOptions.ApiKey));
            services.AddSingleton(serviceClient);

            services.AddDbContext<AltoDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Sql"), b => b.MigrationsAssembly("Alto.Web"))
                       .ConfigureWarnings(x => x.Log());

                if (CurrentEnvironment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"));
            services.AddTransient(_ => redis.GetDatabase());
            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(365);
            });
            //paypal compatibility issue
            ConfigurationManager.AppSettings["PayPalLogger"] = "Alto.Logic.PayPal.PayPalLogger, Alto.Logic";
            ConfigurationManager.AppSettings["PayPalLogger.Delimiter"] = ";";

            services.AddSingleton<IGoogleAnalyticsLogic, GoogleAnalyticsLogic>();
            services.Configure<PayPalOptions>(Configuration);
            services.Configure<IntercomOptions>(Configuration);
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddIdentity<AltoUser, Role>(option =>
                {
                    option.Password.RequireDigit = false;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequiredLength = 1;
                    option.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(365);
                    option.Cookies.ApplicationCookie.AccessDeniedPath = "/Account/Login";
                })
                .AddEntityFrameworkStores<AltoDbContext, int>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<AltoUser>>(EmailConfirmationTokenProviderName);

          
            services.AddSingleton(cloudStorageAccount);

            var config = AutoMapperInitializer.Init();
            services.AddSingleton(config);
            services.AddSingleton(config.CreateMapper());
            services.AddMvc();
            services.AddMemoryCache();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            AddLogicDependencies(services);
        }

        private void AddLogicDependencies(IServiceCollection services)
        {
            services.AddTransient<IHomeLogic, HomeLogic>();
            services.AddTransient<IBenefitLogic, BenefitLogic>();
            services.AddTransient<IProductLogic, ProductLogic>();
            services.AddTransient<IImageLogic, ImageLogic>();
            services.AddTransient<IBlobUploader, AzureUploader>();
            services.AddTransient<IEmailLogic, EmailLogic>();
            services.AddTransient<ICategoryLogic, CategoryLogic>();
            services.AddTransient<IEmailSender, SendGridSender>();
            services.AddTransient<ISmsSender, SendGridSender>();
            services.AddSingleton<PayPalLogic>();
            services.AddTransient<RegistrationPromoSessionManager>();
            services.AddTransient<MembershipManager>();
            services.AddTransient<YoocanUsersManager>();
            services.AddSingleton(_ => new SharedUserTokenManager(Configuration.GetValue<string>("YoocanTokenKey")));
            services.AddTransient<AccountHelper>();
            services.AddTransient<RedisWrapper>();
            services.AddTransient<SearchLogic>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseIdentity();
            app.UseCookieAuthentication();
            loggerFactory.AddSerilog();
            app.EnrichLogger();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error{0}");
            app.UseStaticFiles(new StaticFileOptions
                               {
                                   OnPrepareResponse = context =>
                                       context.Context.Response.Headers.Add("Cache-Control", "public, max-age=2592000")
                               });

            ConfigureFacebookAuth(app);
            ConfigureGoogleAuth(app);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureFacebookAuth(IApplicationBuilder app)
        {
            app.UseFacebookAuthentication(new FacebookOptions
                                          {
                                              AppId = CurrentEnvironment.IsDevelopment() ? "741955015961029" : "325431067856632",
                                              AppSecret = CurrentEnvironment.IsDevelopment() ? "1d6bbe55cb1c5e32bd658ed93502f5e1" : "0caa50f664bd525e9cfba9da65d2ec33",
                                              SaveTokens = true,
                                              Fields = {"picture.width(200).height(200),gender,age_range,birthday"},
                                              Events = new OAuthEvents
                                                       {
                                                           OnCreatingTicket = context =>
                                                           {
                                                               var picture = context.User.Value<JToken>("picture");
                                                               if (picture != null && !picture["data"]["is_silhouette"].Value<bool>())
                                                               {
                                                                   context.Identity.AddClaim(new Claim("picture", picture["data"]["url"].Value<string>()));
                                                               }

                                                               return Task.FromResult(0);
                                                           },
                                                           OnRemoteFailure = context =>
                                                           {
                                                               context.Response.Redirect("/Account/Login");
                                                               context.HandleResponse();

                                                               return Task.FromResult(0);
                                                           }
                                                       }
                                          }
            );
        }

        private void ConfigureGoogleAuth(IApplicationBuilder app)
        {
            app.UseGoogleAuthentication(new GoogleOptions
                                        {
                                            ClientId = CurrentEnvironment.IsDevelopment() ? "611820575922-52ndohcv3l8ngn0qlciahtmtt23mojdq.apps.googleusercontent.com" : "962270450424-ef25gg3u7ro9sv4b8drh93fcq4m773jr.apps.googleusercontent.com",
                                            ClientSecret = CurrentEnvironment.IsDevelopment() ? "pY7QBKiXwN4wxtmUjCrEsy6d" : "_w92vWXvb44rj4ykqZmOrItc",
                                            SaveTokens = true,
                                            Events = new OAuthEvents
                                                     {
                                                         OnCreatingTicket = context =>
                                                         {
                                                             var image = context.User.Value<JObject>("image");
                                                             if (image?["url"] != null)
                                                             {
                                                                 var url = image["url"].Value<string>().Replace("?sz=50", "?sz=200");
                                                                 context.Identity.AddClaim(new Claim("picture", url));
                                                             }

                                                             return Task.FromResult(0);
                                                         },
                                                         OnRemoteFailure = context =>
                                                         {
                                                             context.Response.Redirect("/Account/Login");
                                                             context.HandleResponse();

                                                             return Task.FromResult(0);
                                                         }
                                                     }
                                        }
            );
        }
    }
}