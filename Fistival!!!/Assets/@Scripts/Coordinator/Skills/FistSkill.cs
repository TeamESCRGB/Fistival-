using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Skills
{
    public class FistSkill : SkillCoordinatorBase
    {
        [SerializeField]
        public int _strongDamageMultiplier=2;
        public event Action<int> OnAttack;

        public void Attack(AttackStatus attackStatus, int objectDmg)
        {
            

            var enemies = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, _attackableLayers);

            int totalDmg = _baseDamage;
            if (attackStatus == AttackStatus.STRONG)
            {
                totalDmg *= _strongDamageMultiplier;
            }

            totalDmg += objectDmg;

            if (enemies is null)
            {
                return;
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    continue;
                }
                Managers.Instance.AttackManager.RequestAttack(comp, this, totalDmg);
            }
        }

        public override bool Act(IAttackable target, int calculatedDamage)
        {
            if(target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.StartInvincibleTime();
            }
            OnAttack?.Invoke(calculatedDamage);
            return true;
        }
    }
}