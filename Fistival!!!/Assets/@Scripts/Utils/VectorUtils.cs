using UnityEngine;

namespace Utils
{
    public static class VectorUtils
    {
        public static Vector2 GetDirVec2(in Vector2 start, in Vector2 end)
        {
            return (start - end).normalized;
        }

        public static Vector2 RotateByRad(this in Vector2 vec, float rad)
        {
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);

            return new Vector2  (cos * vec.x - sin * vec.y,
                                 sin * vec.x + cos * vec.y);
        }
    }
}
