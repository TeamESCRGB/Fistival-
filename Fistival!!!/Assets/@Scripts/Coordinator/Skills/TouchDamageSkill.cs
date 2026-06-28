using Coordinator.Victims;
using Manager;
using UnityEngine;

namespace Coordinator.Skills
{
    public class TouchDamageSkill : SkillCoordinatorBase
    {
        private void OnDisable()
        {
            ResetOnAttack();
        }

        protected float _knockbackForce = 1;

        public void Init(int attackableLayers, int baseDamage, float baseStunTime, float knockbackForce)
        {
            _knockbackForce=knockbackForce;
            base.Init(attackableLayers, baseDamage, baseStunTime);
        }


        private void Update()
        {
            var col = Physics2D.OverlapBox(transform.position, transform.localScale, 0, _attackableLayers);
            if(col == null)
            {
                return;
            }
            var go = col.gameObject;
            if(go == null)
            {
                return;
            }

            if (((1 << go.layer) & _attackableLayers) == 0 || go.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            var force = transform.forward.z * _baseDamage * _knockbackForce;
            Managers.Instance.AttackManager.RequestAttack(comp, this, _baseDamage, new Vector2(force, force), _baseStunTime);
        }


        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}