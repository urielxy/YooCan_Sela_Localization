using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Yooocan.WebJobs.Jobs;
using Yooocan.Infrastructure;

namespace Yooocan.WebJobs
{
    class Program
    {
        static void Main()
        {
            var configurationHelper = new ConfigurationHelper();
            var services = configurationHelper.ConfigureServices();

            services.AddTransient<AmazonSyncJob>();
            services.AddTransient<ImagesResizerJob>();
            services.AddTransient<FacebookUsersSyncJob>();

            var container = services.BuildServiceProvider();
            configurationHelper.ConfigureLogger(container);
            var config = new JobHostConfiguration
            {
                JobActivator = new JobActivator(container),
                DashboardConnectionString = configurationHelper.Configuration.GetConnectionString("AzureWebJobsDashboard"),
                StorageConnectionString = configurationHelper.Configuration.GetConnectionString("AzureWebJobsStorage"),
                LoggerFactory = container.GetService<ILoggerFactory>()
            };
            config.UseTimers();

            if (configurationHelper.IsDevelopment())
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            host.RunAndBlock();
        }        
    }
}
