using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Yooocan.Logic.Amazon;

namespace Yooocan.WebJobs.Jobs
{
    public class AmazonSyncJob
    {
        private ILogger<AmazonSyncJob> Logger { get; }
        public AmazonLogic AmazonLogic { get; }

        public AmazonSyncJob(ILogger<AmazonSyncJob> logger, AmazonLogic amazonLogic)
        {
            Logger = logger;
            AmazonLogic = amazonLogic;
        }

        public void Run([TimerTrigger("0 0 */8 * * *", RunOnStartup = false)] TimerInfo timerInfo)
        {
            var result = AmazonLogic.RefreshData();
            Logger.LogInformation($"{result.ChangedImages} product images changed, {result.OutOfStockProducts} out of stock products found");
        }
    }
}
