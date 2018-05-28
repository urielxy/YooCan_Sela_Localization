using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Logic.Messaging;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string userId, string email, string subject, string message, string category, string notificationId)
        {
            //todo: Log to somewhere...
            return Task.CompletedTask;
        }

        public Task<bool> SendEmailAsync(SendEmailModel sendEmail, IEnumerable<SendEmailPersonalizationModel> personalizations)
        {
            return Task.FromResult(true);
        }
    }
}