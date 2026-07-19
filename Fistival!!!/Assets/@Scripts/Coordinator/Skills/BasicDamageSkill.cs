using Coordinator.Victims;
using UnityEngine;

namespace Coordinator.Skills
{
    public class BasicDamageSkill : SkillCoordinatorBase
    {
        private void OnDisable()
        {
            ResetOnAttack();
        }
        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage, transform.position, true);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}