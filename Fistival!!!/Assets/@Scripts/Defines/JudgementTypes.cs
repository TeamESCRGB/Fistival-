using System;

namespace Defines
{
    [Flags]
    public enum JudgementTypes : byte
    {
        PERFECT = 1<<0,
        GOOD = 1<<1,
        EARLY_MISS=1<<2,
        LATE_MISS=1<<3
    }
}
