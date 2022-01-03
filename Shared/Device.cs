using System;

namespace Cryptonyms.Shared
{
    public record Device
    {
        public string DeviceId { get; init; }

        public DateTime LastSeenUtc { get; init; }
    }
}