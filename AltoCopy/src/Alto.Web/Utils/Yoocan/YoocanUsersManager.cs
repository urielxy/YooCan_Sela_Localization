using System;
using System.Text;
using Alto.Logic.Utils.Shared;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Alto.Web.Utils.Yoocan
{
    public class YoocanUsersManager
    {
        private readonly HttpContext _httpContext;
        private readonly ILogger<YoocanUsersManager> _logger;
        private readonly SharedUserTokenManager _userTokenManager;
        private readonly IDataProtector _dataProtector;

        private const string YoocanUserCookieName = "YoocanUser";

        public YoocanUsersManager(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectorProvider,
            ILogger<YoocanUsersManager> logger, SharedUserTokenManager userTokenManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _logger = logger;
            _userTokenManager = userTokenManager;
            _dataProtector = dataProtectorProvider.CreateProtector("Alto.Web.Utils.YoocanUsersManager");
        }

        public bool TrySetYoocanUserCookie(string encryptedUserToken)
        {
            SharedUserToken userToken;
            try
            {
                userToken = _userTokenManager.DecryptToken(encryptedUserToken);
            }
            catch (Exception e)
            {
                _logger.LogError(5115124, e, "error decrypting yoocan user token");
                return false;
            }
            var cookie = CreateYoocanUserCookie(userToken);
            _httpContext.Response.Cookies.Append(YoocanUserCookieName, cookie,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(24) });
            return true;
        }

        private string CreateYoocanUserCookie(SharedUserToken userToken)
        {
            var session = new YoocanUserCookie
            { 
                UserName = userToken.UserName,
                WasAuthorized = !string.IsNullOrEmpty(userToken.UserName),
                CookieCreationDate = DateTimeOffset.Now,
                TokenCreationDate = userToken.CreationDate,
                Guid = Guid.NewGuid()
            };

            var json = JsonConvert.SerializeObject(session);
            var encrypted = _dataProtector.Protect(Encoding.UTF8.GetBytes(json));
            var token = Convert.ToBase64String(encrypted);
            return token;
        }

        public YoocanUserCookie GetYoocanUserCookie()
        {
            var token = _httpContext.Request.Cookies[YoocanUserCookieName];
            if (token == null)
                return null;

            YoocanUserCookie cookie;
            try
            {
                var encrypted = Convert.FromBase64String(token);
                var decrypted = _dataProtector.Unprotect(encrypted);
                var json = Encoding.UTF8.GetString(decrypted);
                cookie = JsonConvert.DeserializeObject<YoocanUserCookie>(json);
            }
            catch (Exception e)
            {
                _logger.LogError(5215124, e, "error decrypting yoocan user cookie");
                DeleteCookie();
                return null;
            }

            return cookie;
        }

        public void DeleteCookie()
        {
            _httpContext.Response.Cookies.Delete(YoocanUserCookieName);
        }
    }
}