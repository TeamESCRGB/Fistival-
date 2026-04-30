using Data;
using UnityEngine;
using static Utils.VectorUtils;

namespace Coordinator
{
    public abstract class GuidedProjectileCoordinator : ProjectileCoordinator
    {
        private Transform _target;
        private Vector3 _tarPos;
        public void Init(LayerMask attackableLayerMask,Transform target ,ProjectileData data)
        {
            Init(attackableLayerMask, data);
            _target = target;
        }


        protected override void OnFixedUpdate()
        {
            if(_target != null)
            {
                _tarPos = _target.position;
                Vector2 dir = _tarPos - transform.position;
                _projActor.LookDir(dir);
                float curRad = Mathf.Atan2(_rb2d.linearVelocityY, _rb2d.linearVelocityX);
                float tarRad = Mathf.Atan2(dir.y, dir.x);
                _rb2d.linearVelocity = _rb2d.linearVelocity.RotateByRad(Mathf.DeltaAngle(curRad * Mathf.Rad2Deg, tarRad * Mathf.Rad2Deg) * Mathf.Deg2Rad);
            }

            base.OnFixedUpdate();

        }
    }
}
