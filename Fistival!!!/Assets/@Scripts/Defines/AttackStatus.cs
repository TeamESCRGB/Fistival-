using System;

namespace Defines
{
    [Flags]
    public enum AttackStatus : byte
    {
        NO_PRESSED=1,
        PRESSED=2,
        STRONG_RDY=4,
        STRONG=8,
        WEAPON=16
    }
}
