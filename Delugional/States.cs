using System;

namespace Delugional
{
    [Flags]
    public enum States
    {
        Downloading = 1 << 1,
        Seeding = 1 << 2,
        Active = 1 << 3,
        Paused = 1 << 4,
        Queued = 1 << 5,
        All = ~0,
    }
}