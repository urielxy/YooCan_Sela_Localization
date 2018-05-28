using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace Alto.Logic.PayPal
{
    public class PayPalLogic
    {
        public PayPalOptions Options { get; }
        public PayPalLogic(IOptions<PayPalOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public APIContext GetContext()
        {
            var config = new Dictionary<string, string>
            {
                {"mode", Options.PayPalIsProduction ? "live" : "sandbox"}
            };
            var token = new OAuthTokenCredential(Options.PayPalClientId, Options.PayPalSecret, config).GetAccessToken();
            var context = new APIContext(token)
            {
                Config = config
            };
            return context;
        }
    }
}
