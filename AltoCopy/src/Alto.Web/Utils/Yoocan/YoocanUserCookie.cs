using System;

namespace Alto.Web.Utils.Yoocan
{
    public class YoocanUserCookie
    {
        public bool WasAuthorized { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset TokenCreationDate { get; set; }
        public DateTimeOffset CookieCreationDate { get; set; }
        public Guid Guid { get; set; }
    }
}
