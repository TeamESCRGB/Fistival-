using Coordinator;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator.Projectiles
{
    public class SwordSlashProjectile : ProjectileCoordinator
    {
        private LayerMask _attackableMask;
        private LayerMask _explodableMask;
        private float _stunTime;

        public override void Init(LayerMask attackableLayerMask, ProjectileData data)
        {
            base.Init(attackableLayerMask, data);
            _stunTime = data.StunTime;
            _explodableMask = data.ExplodableLayerMask;
            _attackableMask = attackableLayerMask;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var tarLayer = 1 << collision.gameObject.layer;
            if((tarLayer&_attackableMask) != 0 && collision.gameObject.TryGetComponent<IAttackable>(out var comp))
            {
                Managers.Instance.AttackManager.RequestAttack(comp,_skill, _skill.GetBaseDamage, _rb2d.linearVelocity*_explosionKnockBack, _stunTime);
            }

            if((tarLayer & _explodableMask) != 0)
            {
                Destruct();
            }
        }

        protected override bool CanExplode()
        {
            return false;
        }

        protected override void OnFixedUpdate()
        {
            
        }

        protected override void OnExplode()
        {
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }
    }
}
