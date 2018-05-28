namespace Alto.Web.Utils
{
    public interface IGoogleAnalyticsLogic
    {
        void TrackEvent(int userId, string category, string action, string label, int? value = null);
        void TrackPageview(int userId, string path);
    }

    public class MockGoogleAnalyticsLogic : IGoogleAnalyticsLogic
    {
        public void TrackEvent(int userId, string category, string action, string label, int? value = null)
        {
        }

        public void TrackPageview(int userId, string path)
        {
        }
    }
}