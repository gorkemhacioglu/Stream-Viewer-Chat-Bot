namespace BotCore.Dto
{
    public class StreamService
    {
        public enum Service
        {
            Twitch,
            Youtube,
            DLive,
            NimoTv,
            Twitter,
            Facebook
        }

        public Service ServiceType { get; set; }
    }
}