using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models;

namespace Yooocan.Logic.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string userId, string email, string subject, string message, string category, string notificationId);
        Task<bool> SendEmailAsync(SendEmailModel sendEmail, IEnumerable<SendEmailPersonalizationModel> personalizations);
    }
}
