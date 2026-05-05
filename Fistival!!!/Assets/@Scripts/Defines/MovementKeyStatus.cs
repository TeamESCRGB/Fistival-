using System;

namespace Defines
{
    [Flags]
    public enum MovementKeyStatus : sbyte
    {
        RIGHT = 1<<0,
        LEFT = 1<<1,
        UP = 1<<2,
        DOWN= 1<<3,
    }
}