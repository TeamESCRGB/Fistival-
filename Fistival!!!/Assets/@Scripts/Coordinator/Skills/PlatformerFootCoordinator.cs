using Coordinator.Victims;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Skills
{
    public class PlatformerFootCoordinator : SkillCoordinatorBase
    {
        private Transform _box;
        public Action OnStepKill;

        private void Awake()
        {
            _box = transform;
        }

        public override bool Act(IAttackable target, int calculatedDamage)
        {
            if(target.CanAttack() == false)
            {
                return false;
            }
            target.TakeDamage(calculatedDamage);
            return true;
        }


        private void FixedUpdate()
        {
            var enemies = Physics2D.OverlapBoxAll(_box.position, _box.localScale,0,_attackableLayers);

            if(enemies is null)
            {
                return;
            }

            for(int i = 0; i < enemies.Length; i++)
            {
                var enemy = enemies[i];
                if (enemy.TryGetComponent<IAttackable>(out var target) && CanAttackTarget(target))
                {
                    Managers.Instance.AttackManager.RequestAttack(target, this, _baseDamage);
                    OnStepKill?.Invoke();
                }
            }
        }
    }
}