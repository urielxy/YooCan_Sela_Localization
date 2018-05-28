using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alto.Models.Messaging;

namespace Alto.Logic.Messaging
{
    public class EmailLogic : IEmailLogic
    {
        private readonly IEmailSender _emailSender;

        public EmailLogic(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<bool> SendResetPasswordEmailAsync(EmailUserData emailUserData, string resetPasswordUrl, bool isForgetPassword)
        {
            var sendEmail = new SendEmailModel
            {
                From = "info@altolife.com",
                FromName = "Team Alto",
                To = emailUserData.Email,
                Category = "Reset password",
                Subject = "",
                TemplateId = isForgetPassword ? "a5b6542f-b339-4359-bd3d-a0a6953c64e6" : "02d59dba-2b1e-4a25-8456-cebd25e66faf",
                BypassListManagement = true
            };

            var personalizations = new List<SendEmailPersonalizationModel>
            {
                new SendEmailPersonalizationModel
                {
                    Email = emailUserData.Email,
                    UserId = emailUserData.UserId,
                    Substitutions =
                        new Dictionary<string, string>
                        {
                            {"{callbackUrl}", resetPasswordUrl},
                            {"{firstName}", emailUserData.FirstName ?? "there"},
                            {"{email}", emailUserData.Email}
                        }
                }
            };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task<bool> SendConfirmEmailAsync(string email, int userId, string callbackUrl)
        {
            var sendEmail = new SendEmailModel
            {
                To = email,
                Category = "Confirm your email",
                Subject = "",
                TemplateId = "f8d4ad16-1736-4e78-9b6e-56b787f81b6a",
                BypassListManagement = true,
                From = "info@altolife.com",
                FromName = "Team Alto"
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

        public async Task<bool> SendOrderConfirmationEmailAsync(OrderConfirmationData data)
        {
            var sendEmail = new SendEmailModel
            {
                To = data.Email,
                Category = "Order confirmation",
                Subject = "Order confirmation",
                TemplateId = "0ee66094-283b-4263-910b-a84a2edbf1a0",
                BypassListManagement = true,
                From = "info@altolife.com",
                FromName = "Alto team"
            };

            var personalizations = new List<SendEmailPersonalizationModel>
            {
                new SendEmailPersonalizationModel
                {
                    Email = data.Email,
                    UserId = data.UserId,
                    Substitutions =
                        new Dictionary<string, string>
                        {
                            {"{firstName}", data.FirstName},
                            {"{productImage}", data.ProductImage},
                            {"{productName}", data.ProductName},
                            {"{variationsText}", data.VariationsText},
                            {"{productPrice}", data.ProductPrice},
                            {"{shippingPrice}", data.ShippingPrice}
                        }
                }
            };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task<bool> SendMembershipConfirmationEmailAsync(MemberConfirmationData data)
        {
            var sendEmail = new SendEmailModel
            {
                To = data.Email,
                Category = "Membership confirmation",
                Subject = "Alto Membership Confirmation",
                TemplateId = "0084240d-6769-4c98-b327-491874a03198",
                BypassListManagement = true,
                From = "info@altolife.com",
                FromName = "Alto team"
            };

            var personalizations = new List<SendEmailPersonalizationModel>
            {
                new SendEmailPersonalizationModel
                {
                    Email = data.Email,
                    UserId = data.UserId,
                    Substitutions =
                        new Dictionary<string, string>
                        {
                            {"{firstName}", data.FirstName},
                            {"{lastName}", data.LastName},
                            {"{membershipDuration}", data.MembershipDuration},
                            {"{membershipExpiration}", data.MembershipExpiration.ToLongDateString()}
                        }
                }
            };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }

        public async Task<bool> SendPostAccountCreationEmailAsync(string email, int userId, string continueRegistrationUrl, string firstName = "there", bool hasFreeTrial = false)
        {
            var sendEmail = new SendEmailModel
            {
                To = email,
                Category = "Account creation confirmation",
                Subject = "Welcome to Alto!",
                TemplateId = hasFreeTrial ? "f9ac7fb0-514c-404f-b976-0e9fd5dccabf" : "e9835291-f57d-4594-9049-2c144b04e0ba",
                BypassListManagement = true,
                From = "info@altolife.com",
                FromName = "Alto team"
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
                            {"{firstName}", firstName},
                            {"{continueRegistrationUrl}", continueRegistrationUrl}
                        }
                }
            };

            return await _emailSender.SendEmailAsync(sendEmail, personalizations);
        }
    }

    public class OrderConfirmationData
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public string VariationsText { get; set; }
        public string ProductPrice { get; set; }
        public string ShippingPrice { get; set; }
    }

    public class MemberConfirmationData
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MembershipDuration { get; set; }
        public DateTime MembershipExpiration { get; set; }
    }
}