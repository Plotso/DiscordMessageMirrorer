using System.Collections.Generic;

namespace MessageMirrorerBot
{
    public class BotConfig
    {
        public string Token { get; set; }

        public char CommandPrefix { get; set; }

        public IEnumerable<MirrorRule> Rules { get; set; }
    }
}
