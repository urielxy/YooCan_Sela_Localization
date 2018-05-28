using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yooocan.Dal;
using Yooocan.Logic.Options;

namespace Yooocan.WebJobs.Jobs
{
    public class FacebookUsersSyncJob
    {
        private ILogger<FacebookUsersSyncJob> Logger { get; }
        public ApplicationDbContext Context { get; }
        public HttpClient HttpClient { get; }
        public FacebookKeys FacebookKeys { get; }

        public FacebookUsersSyncJob(ILogger<FacebookUsersSyncJob> logger, ApplicationDbContext context, HttpClient httpClient, IOptions<FacebookKeys> facebookOptions)
        {
            Logger = logger;
            Context = context;
            HttpClient = httpClient;
            FacebookKeys = facebookOptions.Value;
        }

        [NoAutomaticTrigger]
        public async Task Run()
        {
            var facebookUsers = await Context.UserLogins.Where(x => x.LoginProvider == "Facebook").AsNoTracking().ToListAsync();
            var facebookUsersIds = facebookUsers.Select(x => x.UserId).ToList();
            var notSyncedfacebookUsers = await Context.Users.Where(x => (x.Gender == null) && facebookUsersIds.Contains(x.Id)).ToListAsync();
            var syncedUsersCount = 0;
            var failedUsersCount = 0;
            foreach (var user in notSyncedfacebookUsers)
            {
                try
                {
                    var appscopedId = facebookUsers.Single(x => x.UserId == user.Id).ProviderKey;
                    var facebookUserDataUrl = $"https://graph.facebook.com/v2.11/{appscopedId}?access_token={FacebookKeys.AppId}|{FacebookKeys.AppSecret}&fields=gender";
                    var response = await HttpClient.GetStringAsync(facebookUserDataUrl);
                    var parsedResponse = JsonConvert.DeserializeObject<GraphApiResponse>(response);
                    switch (parsedResponse.Gender)
                    {
                        case "male":
                            user.Gender = Entities.Gender.Male;
                            break;
                        case "female":
                            user.Gender = Entities.Gender.Female;
                            break;
                        case null:
                        case "":
                            break;
                        default:
                            throw new InvalidOperationException($"unknown gender value: {parsedResponse.Gender}");
                    }
                    await Context.SaveChangesAsync();
                    syncedUsersCount++;
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e, "Unable to sync data for UserId: {userId}", user.Id);
                    failedUsersCount++;
                }
            }
            Logger.LogInformation($"synced {syncedUsersCount} facebook users. failed to sync {failedUsersCount} users");
        }

        public class GraphApiResponse
        {
            public string Gender { get; set; }
        }
    }
}
