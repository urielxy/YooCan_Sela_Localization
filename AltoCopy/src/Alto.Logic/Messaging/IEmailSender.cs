using System.Collections.Generic;
using System.Threading.Tasks;
using Alto.Models.Messaging;

namespace Alto.Logic.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(int userId, string email, string subject, string message, string category, string notificationId);
        Task<bool> SendEmailAsync(SendEmailModel sendEmail, IEnumerable<SendEmailPersonalizationModel> personalizations);
    }
}
