using Coordinator;
using Coordinator.Victims;
using UnityEngine;

public class TestSkill : SkillCoordinatorBase
{
    public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback,float stun)
    {
        if(target.CanAttack())
        {
            //target.TakeDamage(calculatedDamage);
            target.TakeKnockBack(knockback);
            
        }
        Debug.Log($"{calculatedDamage}dmg, {knockback}");
        return true;
    }
}
