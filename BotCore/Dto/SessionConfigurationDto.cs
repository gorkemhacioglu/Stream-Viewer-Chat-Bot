using Microsoft.Playwright;

namespace BotCore.Dto
{
    public class SessionConfigurationDto
    {
        public string Url { get; set; }

        public int Count { get; set; }

        public string PreferredQuality { get; set; }

        public LoginDto LoginInfo { get; set; }

        public StreamService.Service Service { get; set; }

        public Proxy Proxy { get; set; }
    }
}