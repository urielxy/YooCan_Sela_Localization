namespace Yooocan.Web.Utils
{
    public class ClientHelper
    {
        public static bool IsMobileDevice(string userAgent)
        {
            var mobileDevice = !string.IsNullOrEmpty(userAgent) &&
                               userAgent.ToLower().Contains("mobi");
            return mobileDevice;
        }
    }
}
