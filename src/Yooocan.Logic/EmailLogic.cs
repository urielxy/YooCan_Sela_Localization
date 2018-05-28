using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Yooocan.Logic.Extensions;
using Yooocan.Logic.Messaging;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public class EmailLogic : IEmailLogic
    {
        private readonly IEmailSender _emailSender;

        public EmailLogic(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<bool> SendResetPasswordEmailAsync(EmailUserData emailUserData, string resetPasswordUrl)
        {
            var sendEmail = new SendEmailModel
                            {
                                To = emailUserData.Email,
                                Category = "Reset password",
                                Subject = "",
                                TemplateId = "15eac1e8-9f51-48e0-8fd6-843aebf318ed",
                                BypassListManagement = true
                            };

            var personalizations = new List<SendEmailPersonalizationModel>
                                   {
                                       new SendEmailPersonalizationModel
                                       {
                                           Name = $"{emailUserData.FirstName} {emailUserData.LastName}",
                                           Email = emailUserData.Email,
                                           UserId = emailUserData.UserId,
                                           Substitutions =
                                               new Dictionary<string, string>
                                               {
                                                   {"{resetPasswordUrl}", resetPasswordUrl},
                                                   {"{firstName}", $" {emailUserData.FirstName}"},
                                                   {"{email}", emailUserData.Email},
                                                   {
                                                       "{profileImageUrl}", emailUserData.ProfileImageUrl ??
                                                                            "https://static.licdn.com/scds/common/u/images/themes/katy/ghosts/person/ghost_person_100x100_v1.png"
                                                   }
                                               }
                                       }
                                   };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task<bool> SendYourStoryWasPublishedEmailAsync(EmailUserData emailUserData, string storyUrl, string storyTitle, int storyId)
        {
            var sendEmail = new SendEmailModel
                            {
                                To = emailUserData.Email,
                                Category = "Your story was published",
                                Subject = "",
                                TemplateId = "0fdecdf4-4cb6-4e7b-b7db-1bd04f51f4f5",
                                NotificationId = $"YourStoryWasPublished_{storyId}",
                                BypassListManagement = true,
                                From = "jessica@yoocantech.com",
                                FromName = "Team yoocan"
                            };

            var personalizations = new List<SendEmailPersonalizationModel>
                                   {
                                       new SendEmailPersonalizationModel
                                       {
                                           Name = $"{emailUserData.FirstName} {emailUserData.LastName}",
                                           Email = emailUserData.Email,
                                           UserId = emailUserData.UserId,
                                           Substitutions =
                                               new Dictionary<string, string>
                                               {
                                                   {"{storyUrl}", storyUrl},
                                                   {"{storyUrlEncoded}", WebUtility.UrlEncode(storyUrl)},
                                                   {"{storyTitle}", storyTitle},
                                                   {"{firstName}", string.IsNullOrEmpty(emailUserData.FirstName) ? "" : $" {emailUserData.FirstName}"},
                                                   {"{email}", emailUserData.Email},
                                                   {
                                                       "{profileImageUrl}", emailUserData.ProfileImageUrl ??
                                                                            "https://static.licdn.com/scds/common/u/images/themes/katy/ghosts/person/ghost_person_100x100_v1.png"
                                                   }
                                               }
                                       }
                                   };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task<bool> SendStoryOfTheDayAsync(List<EmailUserData> emailUserDatas, StoryOfTheDayData storyData)
        {
            var isDaily = DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday;
            var emailType = isDaily ? "Daily" : "Weekly";
            var asmGroup = isDaily ? 1639 : 1665;

            var sendEmail = new SendEmailModel
                            {
                                To = "aaa@aaa.com",
                                Category = $"The {emailType} story",
                                Subject = $"The {emailType} story",
                                TemplateId = "9b56d7b8-9a0a-4c05-becc-e44884d4de70",
                                NotificationId = $"The {emailType} story{storyData.StoryId}",
                                BypassListManagement = false,
                                AsmGroupId = asmGroup,
                                From = "jessica@yoocantech.com",
                                FromName = "Team yoocan",
                                SendAt = emailUserDatas.Count == 1 ? (DateTime?) null : DateTime.Today.ToUniversalTime().AddHours(14)
                            };
            var personalizationsChunks = emailUserDatas.Select(userData => new SendEmailPersonalizationModel
                                                                           {
                                                                               Name = $"{userData.FirstName} {userData.LastName}",
                                                                               Email = userData.Email,
                                                                               UserId = userData.UserId,
                                                                               Substitutions =
                                                                                   new Dictionary<string, string>
                                                                                   {
                                                                                       {"{storyUrl}", storyData.StoryUrl},
                                                                                       {"{storyUrlEncoded}", WebUtility.UrlEncode(storyData.StoryUrl)},
                                                                                       {"{storyTitle}", storyData.StoryTitle},
                                                                                       {"{storyText}", storyData.StoryText},
                                                                                       {"{email}", userData.Email},
                                                                                       {"{emailType}", emailType},
                                                                                       {"{primaryImage}", storyData.PrimaryImage}
                                                                                   }
                                                                           }).ToList().ChunkBy(1000);

            await Task.WhenAll(personalizationsChunks.Select(chunk => _emailSender.SendEmailAsync(sendEmail, chunk)).ToList());
            return true;
        }

        public async Task<bool> SendYourStoryGotCommentEmailAsync(EmailUserData emailUserData, string storyUrl, string storyTitle, int commentId)
        {
            var sendEmail = new SendEmailModel
            {
                To = emailUserData.Email,
                Category = "New comment on story",
                Subject = "Your Story has just empowered someone on yoocan.com!",
                TemplateId = "d2d16d40-8ed7-444e-93c1-998b5bba485f",
                NotificationId = $"YourStoryGotComment{commentId}",
                From = "jessica@yoocantech.com",
                FromName = "Team yoocan"
            };

            var personalizations = new List<SendEmailPersonalizationModel>
                                   {
                                       new SendEmailPersonalizationModel
                                       {
                                           Name = $"{emailUserData.FirstName} {emailUserData.LastName}",
                                           Email = emailUserData.Email,
                                           UserId = emailUserData.UserId,
                                           Substitutions =
                                               new Dictionary<string, string>
                                               {
                                                   {"{storyUrl}", storyUrl},
                                                   {"{storyTitle}", storyTitle},
                                                   {"{firstName}", string.IsNullOrEmpty(emailUserData.FirstName) ? "" : $" {emailUserData.FirstName}"},
                                                   {"{email}", emailUserData.Email},
                                                   {
                                                       "{profileImageUrl}", emailUserData.ProfileImageUrl ??
                                                                            "https://static.licdn.com/scds/common/u/images/themes/katy/ghosts/person/ghost_person_100x100_v1.png"
                                                   }
                                               }
                                       }
                                   };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task SendFollowingPublishedStoryEmailAsync(List<EmailUserData> emailUserDatas, FollowingPublishedStoryUserData storyData)
        {
            var sendEmail = new SendEmailModel
            {
                To = "aaa@aaa.com",
                Category = "Following Published Story",
                Subject = "a",
                TemplateId = "c316b929-ced1-435f-922d-92b39205492b",
                NotificationId = $"FollowingPublishedStory_{storyData.StoryId}",
            };

            var personalizationsChunks = emailUserDatas.Select(userData => new SendEmailPersonalizationModel
                                                                {
                                                                    Name = $"{userData.FirstName} {userData.LastName}",
                                                                    Email = userData.Email,
                                                                    UserId = userData.UserId,
                                                                    Substitutions =
                                                                        new Dictionary<string, string>
                                                                        {
                                                                            { "{authorFirstName}", storyData.AuthorFirstName},
                                                                            { "{authorLastName}", storyData.AuthorLastName},
                                                                            {"{storyUrl}", storyData.StoryUrl},
                                                                            {"{storyTitle}", storyData.StoryTitle},
                                                                            {"{firstName}", string.IsNullOrEmpty(userData.FirstName) ? "" : $" {userData.FirstName}"},
                                                                            {"{email}", userData.Email},
                                                                            {
                                                                                "{profileImageUrl}", userData.ProfileImageUrl ??
                                                                                                     "https://static.licdn.com/scds/common/u/images/themes/katy/ghosts/person/ghost_person_100x100_v1.png"
                                                                            }
                                                                        }
                                                                }).ToList().ChunkBy(1000);
            
            await Task.WhenAll(personalizationsChunks.Select(chunk => _emailSender.SendEmailAsync(sendEmail, chunk)).ToList());
        }

        public async Task<bool> SendConfirmEmailAsync(string email, string userId, string callbackUrl)
        {
            var sendEmail = new SendEmailModel
            {
                To = email,
                Category = "Confirm your email",
                Subject = "",
                TemplateId = "4a4596fa-38eb-4635-8b94-46216e7983ed",
                BypassListManagement = true,
                From = "jessica@yoocantech.com",
                FromName = "Team yoocan"
            };

            var personalizations = new List<SendEmailPersonalizationModel>
                                   {
                                       new SendEmailPersonalizationModel
                                       {
                                           Email = email,
                                           UserId = userId,
                                           Substitutions =
                                               new Dictionary<string, string>
                                               {
                                                   {"{callbackUrl}", callbackUrl}
                                               }
                                       }
                                   };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }
    }
}