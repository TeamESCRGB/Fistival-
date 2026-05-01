using UnityEngine;

namespace Actor
{
    public class ProjectileActor
    {
        private Rigidbody2D _rb2d;

        public ProjectileActor(Rigidbody2D rb2d)
        {
            _rb2d = rb2d;
        }

        public void LookDir(Vector2 dir)
        {
            _rb2d.MoveRotation(Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg);
        }
    }
}