using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Skills
{
    public class FistSkill : SkillCoordinatorBase
    {
        public void Attack(AttackStatus attackStatus, int objectDmg, int strongAttackDamage, float strongStunTime)
        {
            var enemies = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0, _attackableLayers);

            int totalDmg = _baseDamage;
            float stunTime = _baseStunTime;
            if (attackStatus == AttackStatus.STRONG)
            {
                _baseStunTime += strongStunTime;
                totalDmg += strongAttackDamage;
            }

            totalDmg += objectDmg;

            if (enemies is null)
            {
                return;
            }

            Vector2 knockback = new Vector2(transform.forward.z * totalDmg,0);

            for (int i = 0; i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    continue;
                }
                Managers.Instance.AttackManager.RequestAttack(comp, this, totalDmg, knockback, stunTime);
            }
        }
        private void OnDisable()
        {
            ResetOnAttack();
        }
        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if(target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
                CallOnAttack(1,calculatedDamage);
            }
            return true;
        }
    }
}