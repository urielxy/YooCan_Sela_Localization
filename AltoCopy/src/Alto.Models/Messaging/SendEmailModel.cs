using System;
using System.Collections.Generic;

namespace Alto.Models.Messaging
{
    public class SendEmailPersonalizationModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Substitutions { get; set; }
        public long? SendAt { get; set; }
    }

    public class SendEmailModel
    {
        public string NotificationId { get; set; }
        public string Category { get; set; }
        public string TemplateId { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool BypassListManagement { get; set; }
        public int? AsmGroupId { get; set; }
        public DateTimeOffset? SendAt { get; set; }

        public SendEmailModel()
        {
            From = "noreply@yooocan.com";
            FromName = "yooocan";
            Content = "Empty";
        }
    }
}