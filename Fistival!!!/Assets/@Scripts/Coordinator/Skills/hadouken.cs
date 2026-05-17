using Coordinator;
using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Skills
{
    public class Hadouken : SkillCoordinatorBase
    {
        public event Action OnAttackEnd;
        [SerializeField]
        private int _demendedCost = 0;
        [SerializeField]
        private int _projectileIDX;

        public int GetDemendedCost()
        {
            return _demendedCost;
        }

        public void Attack(Vector2 dir)
        {
            ProjectileLaunchHelper.LaunchConstantDir(_attackableLayers, _projectileIDX, transform.position, dir);
            OnAttackEnd?.Invoke();
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback)
        {
            return true;
        }
    }
}
