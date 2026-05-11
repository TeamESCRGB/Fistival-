namespace Defines
{
    public enum GunStatus : sbyte
    {
        OFF=0,
        USE=1<<0,
        RELOAD=1<<1,
        FANNING=1<<2
    }
}
