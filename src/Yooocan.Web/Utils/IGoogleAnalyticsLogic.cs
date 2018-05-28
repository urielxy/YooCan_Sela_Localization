namespace Yooocan.Web.Utils
{
    public interface IGoogleAnalyticsLogic
    {
        void TrackEvent(string userId, string category, string action, string label, int? value = null);
        void TrackPageview(string userId, string path);
    }

    public class MockGoogleAnalyticsLogic : IGoogleAnalyticsLogic
    {
        public void TrackEvent(string userId, string category, string action, string label, int? value = null)
        {
        }

        public void TrackPageview(string userId, string path)
        {
        }
    }
}