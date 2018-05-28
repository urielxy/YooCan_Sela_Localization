using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Yooocan.Logic.Recaptchas
{
    public class RecaptchaApi : IRecaptchaApi
    {
        private class Response
        {
            public bool success { get; set; }
        }

        private readonly string _secretKey;
        private readonly ILogger<RecaptchaApi> _logger;

        public RecaptchaApi(ILogger<RecaptchaApi> logger, IOptions<RecaptchaOptions> options)
        {
            _secretKey = options.Value.RecaptchaSecret;
            _logger = logger;
        }
        public async Task<bool> ValidateAsync(string clientIp, string responseToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={responseToken}&remoteip={clientIp}";
                    var response = await httpClient.PostAsync(url, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Recaptch request failed {token}", responseToken);
                        // Somethign went wrong, it's better to approve the request than failing it.
                        return true;
                    }
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Response>(stringResponse).success;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(213321, e, "Recaptch request failed {token}", responseToken);
                // Somethign went wrong, it's better to approve the request than failing it.
                return true;
            }
        }
    }
}