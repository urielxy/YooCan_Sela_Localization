using System.Threading.Tasks;
using Alto.Models.Messaging;

namespace Alto.Logic.Messaging
{
    public interface IEmailLogic
    {
        Task<bool> SendResetPasswordEmailAsync(EmailUserData emailUserData, string resetPasswordUrl, bool isForgetPassword);
        Task<bool> SendConfirmEmailAsync(string email, int userId, string callbackUrl);
        Task<bool> SendOrderConfirmationEmailAsync(OrderConfirmationData data);
        Task<bool> SendMembershipConfirmationEmailAsync(MemberConfirmationData data);
        Task<bool> SendPostAccountCreationEmailAsync(string email, int userId, string continueRegistrationUrl, string firstName = "there", bool hasFreeTrial = false);
    }
}