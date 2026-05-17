using Coordinator;
using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Assets._Scripts.Coordinator.Skills
{
    public class hadouken : SkillCoordinatorBase
    {
        public event Action<int> OnAttack;
        [SerializeField]
        private int _demendedCost = 0;
        [SerializeField]
        private int _projectileIDX;

        public int GetDemendedCost()
        {
            return _demendedCost;
        }

        public void Attack(AttackStatus attackStatus, int objectDmg, Vector2 dir)
        {


        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.StartInvincibleTime();
            }
            OnAttack?.Invoke(calculatedDamage);
            return true;
        }
    }
}
