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

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if(target.CanAttack() == false)
            {
                return false;
            }
            target.TakeDamage(calculatedDamage, transform.position, true);
            target.TakeKnockBack(knockback);
            target.StunFor(stun);
            target.StartInvincibleTime();
            return true;
        }
        private void OnDisable()
        {
            ResetOnAttack();
        }

        private void FixedUpdate()
        {
            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            var enemies = Physics2D.OverlapBoxAll(_box.position, _box.lossyScale,0,_attackableLayers);

            if(enemies.Length <= 0)
            {
                return;
            }

            OnStepKill?.Invoke();

            for (int i = 0; i < enemies.Length; i++)
            {
                var enemy = enemies[i];
                if (enemy.TryGetComponent<IAttackable>(out var target) && CanAttackTarget(target))
                {
                    Managers.Instance.AttackManager.RequestAttack(target, this, _baseDamage, Vector2.down, _baseStunTime);
                }
            }
        }
    }
}