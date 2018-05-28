using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Utils
{
    public class GoogleAnalyticsLogic : IGoogleAnalyticsLogic
    {
        private static string _resourceId;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILogger<GoogleAnalyticsLogic> _logger;

        public GoogleAnalyticsLogic(ILogger<GoogleAnalyticsLogic> logger, IActionContextAccessor actionContextAccessor, string resourceId = "UA-89819212-1")
        {
            _resourceId = resourceId;
            _actionContextAccessor = actionContextAccessor;
            _logger = logger;
        }
        public void TrackEvent(int userId, string category, string action, string label, int? value = null)
        {
            Track(HitType.@event, userId, category, action, label, value);
        }

        public void TrackPageview(int userId, string path)
        {
            Track(HitType.pageview, userId, path: path);
        }

        private static string GetGoogleClientId(HttpRequest request)
        {            
            try
            {
                string cookie;
                if (request.Cookies.TryGetValue("_ga", out cookie))
                {
                    var match = Regex.Match(cookie, "GA\\d+\\.\\d+\\.(.+)");
                    return match.Groups[1].Value;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Track(HitType type, int userId, string category = null, string action = null, string label = null, int? value = null, string path = null)
        {
            try
            {
                var request = _actionContextAccessor.ActionContext.HttpContext.Request;

                if (type == HitType.@event && string.IsNullOrEmpty(category)) throw new ArgumentNullException(nameof(category));
                if (type == HitType.@event && string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

                var clientId = GetGoogleClientId(request);

                var postData = new Dictionary<string, string>
                                    {
                                        {"v", "1"},
                                        {"tid", _resourceId},
                                        {"cid", clientId ?? Guid.NewGuid().ToString("N")},
                                        {"t", type.ToString()},
                                        {"uid", userId.ToString()},
                                        {"uip", NetworkHelper.GetIpAddress(request)}
                                    };

                if (!string.IsNullOrWhiteSpace(category))
                {
                    postData.Add("ec", category);
                }

                if (!string.IsNullOrWhiteSpace(action))
                {
                    postData.Add("ea", action);
                }

                if (!string.IsNullOrEmpty(label))
                {
                    postData.Add("el", label);
                }
                if (value.HasValue)
                {
                    postData.Add("ev", value.ToString());
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    postData.Add("dp", path);
                    postData.Add("dh", request.Host.Host);
                }

                var httpClient = new HttpClient();
                var referrer = request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referrer))
                    httpClient.DefaultRequestHeaders.Referrer = new Uri(referrer);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(request.Headers["User-Agent"]);

                Task.Run(async () =>
                {
                    try
                    {
                        using (var response = await httpClient.PostAsync("http://www.google-analytics.com/collect", new FormUrlEncodedContent(postData)))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                throw new Exception($"error code: {response.StatusCode} content: {responseContent}");
                            }                            
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(47832, ex, "Logging for GA failed {hitType} {userId} {category} {action} {label} {value}",
                                    type, userId, category, action, label, value);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(47832, ex, "Logging for GA failed {hitType} {userId} {category} {action} {label} {value}",
                    type, userId, category, action, label, value);
            }
        }

        private enum HitType
        {
            // ReSharper disable InconsistentNaming
            @event,
            pageview
            // ReSharper restore InconsistentNaming
        }
    }
}