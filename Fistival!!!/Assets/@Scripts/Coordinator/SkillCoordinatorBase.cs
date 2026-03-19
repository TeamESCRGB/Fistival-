using Coordinator.Victims;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator
{
    public abstract class SkillCoordinatorBase : MonoBehaviour
    {
        private int _attackableLayers = 0;

        private int _baseDamage = 0;

        public void Init(int attackableLayers, int baseDamage)
        {
            _baseDamage = baseDamage;
            _attackableLayers = 0;

            _attackableLayers = attackableLayers;

        }

        public virtual bool CanAttackTarget(IAttackable target)
        {
            return ((target.GetMaskedLayer() & _attackableLayers) != 0) && target.CanAttack();
        }

        public int GetBaseDamage => _baseDamage;

        public abstract bool Act(IAttackable target, int calculatedDamage);
    }
}