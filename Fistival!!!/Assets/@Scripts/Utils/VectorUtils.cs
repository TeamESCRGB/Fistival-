using UnityEngine;

namespace Utils
{
    public static class VectorUtils
    {
        public static Vector2 GetDirVec2(in Vector2 start, in Vector2 end)
        {
            return (start - end).normalized;
        }
    }
}
