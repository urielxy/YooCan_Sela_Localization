using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public interface IEmailLogic
    {
        Task<bool> SendResetPasswordEmailAsync(EmailUserData emailUserData, string resetPasswordUrl);

        Task<bool> SendYourStoryWasPublishedEmailAsync(EmailUserData emailUserData, string storyUrl, string storyTitle, int storyId);

        Task SendFollowingPublishedStoryEmailAsync(List<EmailUserData> emailUserDatas, FollowingPublishedStoryUserData storyData);
        Task<bool> SendConfirmEmailAsync(string email, string userId, string callbackUrl);
        Task<bool> SendYourStoryGotCommentEmailAsync(EmailUserData emailUserData, string storyUrl, string storyTitle, int commentId);
        Task<bool> SendStoryOfTheDayAsync(List<EmailUserData> emailUserDatas, StoryOfTheDayData data);
    }
}