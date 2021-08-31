using Webmilio.Commons.NETCore;

namespace Webmilio.Discord.Test
{
    public class PrivateConfiguration : JsonConfiguration<PrivateConfiguration>
    {
        public string BotToken { get; set; }
    }
}