using UnityEngine;
using static Utils.MovementUtils;

namespace Coordinator
{
    public class GravityProjectileCoordinator : ProjectileCoordinator
    {
        private Vector3 _pos;

        public virtual void Launch(Vector3 initialPos, Vector3 pos)
        {
            _pos = pos;
            transform.position = initialPos;
            Vector2 impulseForce = PreciseCalculateThrowPower(_pos - transform.position, _baseSpeed, _rb2d.linearDamping, 0.001f, Mathf.Abs(Physics2D.gravity.y), _rb2d.linearVelocity);
            _rb2d.AddForce(impulseForce, ForceMode2D.Impulse);
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _projActor.LookDir(_rb2d.linearVelocity);
        }
    }
}