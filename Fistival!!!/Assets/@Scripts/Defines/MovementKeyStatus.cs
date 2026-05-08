using System;

namespace Defines
{
    [Flags]
    public enum MovementKeyStatus : sbyte
    {
        OFF = 0,
        RIGHT = 1<<0,
        LEFT = 1<<1,
        UP = 1<<2,
        DOWN= 1<<3,
        INPUT_LOCK = 1<<4
    }
}