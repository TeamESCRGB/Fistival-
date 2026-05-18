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
        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}