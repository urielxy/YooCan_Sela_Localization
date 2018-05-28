using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic;
using Yooocan.Models.ViewComponents;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Yooocan.Logic.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Yooocan.Entities;

namespace Yooocan.Web.Controllers
{
    public class ChatController : BaseController
    {
        private readonly ICategoriesLogic _categoriesLogic;
        private readonly HttpClient _httpClient;
        private readonly ChatOptions _chatOptions;

        public ChatController(ApplicationDbContext context, IMapper mapper, ICategoriesLogic categoriesLogic, IOptions<ChatOptions> chatOptionsWrapper,
            ILogger<ChatController> logger, UserManager<ApplicationUser> userManager, HttpClient httpClient) : base(context, logger, mapper, userManager)
        {
            _categoriesLogic = categoriesLogic;
            _httpClient = httpClient;
            _chatOptions = chatOptionsWrapper.Value;
        }

        public async Task<IActionResult> GetUserToken()
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(User.FindFirstValue(ClaimTypes.GivenName)))
            {
                return Ok(new ChatModel());
            }
            var userToken = await GetIFlyTokenAsync();
            var model = new ChatModel
            {
                AppId = _chatOptions.AppId,
                UserToken = userToken
            };
            return Ok(model);
        }

        private async Task<string> GetIFlyTokenAsync()
        {
            var requestObject = new
            {
                api_key = _chatOptions.ApiKey,
                app_id = _chatOptions.AppId,
                user_id = "user-" + User.FindFirstValue(ClaimTypes.NameIdentifier),
                user_name = User.FindFirstValue(ClaimTypes.GivenName) + " " + User.FindFirstValue(ClaimTypes.Surname),
                chat_role = User.IsInRole("Admin") ? "admin" : "participant",
                //TODO: should be public profile page - user_profile_url = Url.Action("Edit", "User", null, HttpContext.Request.Scheme), 
                user_avatar_url = User.FindFirstValue("picture")
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.iflychat.com/api/1.1/token/generate", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GenerateTokenResponse>(responseString);
            return responseObject.key;
        }
    }

    public class GenerateTokenResponse
    {
        public string key { get; set; }
        public string expires_in { get; set; }
    }
}
