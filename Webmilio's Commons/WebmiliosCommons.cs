using Webmilio.Commons.Logging;

namespace Webmilio.Commons
{
    public static class WebmiliosCommons
    {
        public static Logger Logger { get; set; } = Logger.Get<CommonOptions>();
    }
}