using System;

namespace Cryptonyms.Shared
{
    public class Device
    {
        public string DeviceId { get; set; }

        public DateTime LastSeenUtc { get; set; }
    }
}