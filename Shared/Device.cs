using System;

namespace Cryptonyms.Shared
{
    public record Device
    {
        public string DeviceId { get; set; }

        public DateTime LastSeenUtc { get; set; }
    }
}