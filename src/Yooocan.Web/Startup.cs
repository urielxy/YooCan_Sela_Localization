using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Yooocan.Logic.Options;
using Yooocan.Logic.Recaptchas;
using Yooocan.Web.Middlewares;
using Yooocan.Web.Policies;
using Yooocan.Web.Utils;
using Yooocan.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web
{
    public class Startup
    {
        private ConfigurationHelper ConfigurationHelper { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public Startup(IHostingEnvironment env)
        {
            ConfigurationHelper = new ConfigurationHelper(env.EnvironmentName, env.ContentRootPath);
            CurrentEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigurationHelper.ConfigureServices(services);

            var configuration = ConfigurationHelper.Configuration;
            services.Configure<SchedulerKeys>(configuration);

            var clientSideApiKeysSection = configuration.GetSection("ClientSideApiKeys");
            var clientSideApiKeys = new ClientSideApiKeys
            {
                IntercomKey = clientSideApiKeysSection["IntercomApiKey"],
                RecaptchaSiteKey = clientSideApiKeysSection["RecaptchaSiteKey"]
            };
            services.AddSingleton(clientSideApiKeys);
            services.Configure<RecaptchaOptions>(configuration);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true; //SECURITY: this may open us to BREACH attack, however I don't think that this version of ASP is vulnerable 
                                               // because the form CSRF tokens seem to change every request.
                                               // Also, before I added this setting, our dynamic views were being gzipped anyway, both locally and on azure, 
                                               // maybe by a component outside the application (posibilities: IIS Express locally, azure reverse proxy)
            });
            services.AddMvc().AddWebApiConventions();

            var authenticationBuilder = services.AddAuthentication();
            ConfigureFacebookAuth(authenticationBuilder, configuration.GetSection("Facebook").Get<FacebookKeys>());
            ConfigureGoogleAuth(authenticationBuilder, configuration.GetSection("GoogleOAuth").Get<GoogleOAuthKeys>());

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MyServiceProvider", policy => policy.Requirements.Add(new MyResourceRequirment("serviceProvider")));
                options.AddPolicy("MyVendor", policy => policy.Requirements.Add(new MyResourceRequirment("vendor")));
            });
            services.AddSingleton<IAuthorizationHandler, MyResourceRequirmentHandler>();
            if (!CurrentEnvironment.IsDevelopment())
            {
                services.AddSingleton<IGoogleAnalyticsLogic, GoogleAnalyticsLogic>();
            }
            else
            {
                services.AddSingleton<IGoogleAnalyticsLogic, MockGoogleAnalyticsLogic>();
            }
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<CsrfHeadersValidationFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfigurationHelper.ConfigureLogger(app.ApplicationServices);
            app.EnrichLogger();
            app.UseAuthentication();
            app.UseMiddleware<IpBlacklistMiddleware>();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error{0}");
            app.UseResponseCompression();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                        context.Context.Response.Headers.Add("Cache-Control", "public, max-age=2592000")
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureFacebookAuth(AuthenticationBuilder builder, FacebookKeys facebookKeys)
        {
            builder.AddFacebook(options => 
            {
                options.AppId = facebookKeys.AppId;
                options.AppSecret = facebookKeys.AppSecret;
                options.SaveTokens = true;
                options.Fields.Add("picture.width(200).height(200),gender");                
                options.Events = new OAuthEvents
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
                };
            }
            );
        }

        private void ConfigureGoogleAuth(AuthenticationBuilder builder, GoogleOAuthKeys googleKeys)
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = googleKeys.AppId;
                options.ClientSecret = googleKeys.AppSecret;
                options.SaveTokens = true;
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        var image = context.User.Value<JObject>("image");
                        if (image != null && image["url"] != null)
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
                };
            }
            );
        }

        //private void ConfigureLinkedinAuth(IApplicationBuilder app)
        //{
        //    app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions
        //    {
        //        ClientId = "77lv43zud67nmp",
        //        ClientSecret = "GE6rkoOW5B2hmlJU",
        //        SaveTokens = true,
        //        Events = new OAuthEvents
        //        {
        //            OnCreatingTicket = context =>
        //            {
        //                var image = context.User.Value<string>("pictureUrl");
        //                if (image != null)
        //                {
        //                    context.Identity.AddClaim(new Claim("picture", image));
        //                }

        //                return Task.FromResult(0);
        //            },
        //            OnRemoteFailure = context =>
        //            {
        //                context.Response.Redirect("/Account/Login");
        //                context.HandleResponse();

        //                return Task.FromResult(0);
        //            }
        //        }
        //    });
        //}
    }
}