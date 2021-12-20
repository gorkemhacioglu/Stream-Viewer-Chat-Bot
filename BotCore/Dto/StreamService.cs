using System;
using System.Collections.Generic;
using System.Text;

namespace BotCore.Dto
{
    public class StreamService
    {

        public Service ServiceType { get; set; }

        public enum Service
        {
            Twitch,
            Youtube
        }
    }
}
