using Coordinator.Victims;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Skills
{
    public class PlatformerFootCoordinator : SkillCoordinatorBase
    {
        private Transform _box;
        public Action OnStepAttackTriedBetweenFixedUpdate;
        private bool _onStepAttackReq;

        private void Awake()
        {
            _box = transform;
        }

        private void Start()
        {
            _onStepAttackReq = false;
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

        private void Update()
        {
            if (Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            var enemies = Physics2D.OverlapBoxAll(_box.position, _box.lossyScale, 0, _attackableLayers);

            if (enemies.Length <= 0)
            {
                return;
            }

            _onStepAttackReq = true;

            for (int i = 0; i < enemies.Length; i++)
            {
                var enemy = enemies[i];
                if (enemy.TryGetComponent<IAttackable>(out var target) && CanAttackTarget(target))
                {
                    Managers.Instance.AttackManager.RequestAttack(target, this, _baseDamage, Vector2.down, _baseStunTime);
                }
            }
        }

        private void FixedUpdate()
        {
            if(_onStepAttackReq)
            {
                _onStepAttackReq = false;
                OnStepAttackTriedBetweenFixedUpdate?.Invoke();
            }
        }
    }
}