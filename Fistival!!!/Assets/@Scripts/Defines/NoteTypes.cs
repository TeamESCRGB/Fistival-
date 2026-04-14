using System;

namespace Defines
{
    [Flags]
    public enum NoteTypes : byte
    {
        SHORT_PARRY_RDY=1<<0,
        SHORT_PARRY=1<<1,
        LONG_PARRY_RDY=1<<2,
        LONG_PARRY_START=1<<3,
        LONG_PARRY_MIDDLE=1<<4,
        LONG_PARRY_END=1<<5,
        NO_ACTION=1<<6
    }
}
