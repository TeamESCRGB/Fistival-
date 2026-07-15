using Coordinator.Mobs;
using Coordinator.Victims;
using Manager;
using UnityEngine;

namespace Coordinator.Objects
{
    public class PaintObjectCoordinator : ObjectCoordinator
    {
        [SerializeField]
        private float _gimmicStunTime=8;
        protected override void InternalCollisionHandler(Collision2D col)
        {
            if (_isThrown == false)
            {
                return;
            }
            var go = col.collider.gameObject;

            if (go.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            var stunTarget = go.GetComponentInParent<CameleonBossMob>();
            if(stunTarget != null)
            {
                stunTarget.PaintStun(_gimmicStunTime);
            }

            if (((1 << go.layer) & _abrasableLayerMask) != 0)
            {
                _durability--;
            }
            
            if (((1 << go.layer) & _attackableLayers) != 0)
            {
                Managers.Instance.AttackManager.RequestAttack(comp, this, (int)(_baseDamage * _rb2d.linearVelocity.magnitude) + _additionalDamage, _rb2d.linearVelocity, _baseStunTime * _chargeRate);
            }

            if (_durability <= 0)
            {
                Managers.Instance.ResourceManager.Destroy(gameObject, true);
            }
        }
    }
}
