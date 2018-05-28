using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.CSharp.HTTP.Client;
using SendGrid.Helpers.Mail;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Models;

namespace Yooocan.Logic.Messaging
{
    public class SendGridSender : IEmailSender, ISmsSender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SendGridSender> _logger;

        public SendGridSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IServiceProvider serviceProvider, ILogger<SendGridSender> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task<bool> SendEmailAsync(SendEmailModel sendEmail, IEnumerable<SendEmailPersonalizationModel> personalizations)
        {
            SendGridAPIClient sg = new SendGridAPIClient(Options.SendGridApiKey);

            Email from = new Email(sendEmail.From, sendEmail.FromName);
            Email to = new Email(sendEmail.To);
            Content content = new Content("text/html", sendEmail.Content);
            var subject = sendEmail.Subject;
            Mail mail = new Mail(from, subject, to, content)
                        {
                            TemplateId = sendEmail.TemplateId,
                            SendAt = sendEmail.SendAt?.ToUnixTimeSeconds(),
                            MailSettings = new MailSettings
                                           {
                                               SandboxMode = new SandboxMode {Enable = Options.SendGridIsSandbox},
                                               BypassListManagement = new BypassListManagement {Enable = sendEmail.BypassListManagement}
                                           }
                        };
            if (sendEmail.AsmGroupId != null)
                mail.Asm = new ASM {GroupId = sendEmail.AsmGroupId};

            mail.AddCategory(sendEmail.Category);
            mail.Personalization = personalizations
                .Select(x => new Personalization
                             {
                                 Tos = new List<Email> {new Email(x.Email, x.Name)},
                                 SendAt = x.SendAt,
                                 Substitutions = x.Substitutions
                             })
                .ToList();

            Response response = await sg.client.mail.send.post(requestBody: mail.Get());
            var isSuccess = ((int) response.StatusCode >= 200) && ((int) response.StatusCode <= 299);

            if (!isSuccess)
                _logger.LogError(123, "Sending email for {category} {notificationId} failed", sendEmail.Category, sendEmail.NotificationId);

            if (sendEmail.NotificationId == null)
                return isSuccess;
            using (var context = _serviceProvider.GetService<ApplicationDbContext>())
            {
                foreach (var personalization in personalizations.Where(x => x.UserId != null))
                {
                    var notificationLog = new NotificationLog
                                          {
                                              IsSuccess = isSuccess,
                                              Method = "Email",
                                              NotificationId = sendEmail.NotificationId,
                                              UserId = personalization.UserId
                                          };
                    context.NotificationLogs.Add(notificationLog);
                }

                await context.SaveChangesAsync();
            }
            return isSuccess;
        }

        public async Task SendEmailAsync(string userId, string email, string subject, string message, string category, string notificationId)
        {
            SendGridAPIClient sg = new SendGridAPIClient(Options.SendGridApiKey);

            Email from = new Email("noreply@yoocantech.com");
            Email to = new Email(email);
            Content content = new Content("text/html", message);
            Mail mail = new Mail(from, subject, to, content);
            mail.AddCategory(category);
            mail.MailSettings = new MailSettings
                                {
                                    SandboxMode = new SandboxMode {Enable = Options.SendGridIsSandbox}
                                };
            mail.AddPersonalization(new Personalization
                                    {

                                        Tos = email.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(x => new Email(x)).ToList(),
                                    });

            Response response = await sg.client.mail.send.post(requestBody: mail.Get());

            if (userId == null || notificationId == null)
                return;

            var notificationLog = new NotificationLog
                                  {
                                      IsSuccess = ((int) response.StatusCode >= 200) && ((int) response.StatusCode <= 299),
                                      Method = "Email",
                                      NotificationId = notificationId,
                                      UserId = userId
                                  };
            using (var context = _serviceProvider.GetService<ApplicationDbContext>())
            {
                context.NotificationLogs.Add(notificationLog);
                await context.SaveChangesAsync();
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}