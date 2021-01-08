using System.Collections.Generic;

namespace MessageMirrorerBot
{
    public class MirrorRule
    {
        public ulong SourceChannelId { get; set; }

        public IEnumerable<ulong> DestinationChannelIds { get; set; }
    }
}
