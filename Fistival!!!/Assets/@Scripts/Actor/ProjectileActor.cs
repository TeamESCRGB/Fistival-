using UnityEngine;

namespace Actor
{
    public class ProjectileActor
    {
        private Rigidbody2D _rb2d;
        private Transform _transform;

        public ProjectileActor(Rigidbody2D rb2d)
        {
            _rb2d = rb2d;
            _transform = rb2d.transform;
        }

        public void LookDir(Vector2 dir)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _rb2d.MoveRotation(angle);
            var rot = _transform.eulerAngles;
            rot.z = angle;
            _transform.eulerAngles = rot;
        }
    }
}