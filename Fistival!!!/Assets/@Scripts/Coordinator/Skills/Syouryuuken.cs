using Coordinator.Movements;
using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Skills
{
    public class Syouryuuken : SkillCoordinatorBase
    {
        public event Action OnAttackEnd;
        [SerializeField]
        private int _demendedCost = 0;
        [SerializeField]
        private Vector2 _force;
        private IPushable _pushable;
        private IAttackable _attackable;
        private float _strongStunTime;

        public void Init(int attackableLayers, int damage, IPushable pushable, IAttackable attackable, float baseStunTime, float strongStunTime)
        {
            base.Init(attackableLayers, damage, baseStunTime);
            _pushable = pushable;
            _attackable = attackable;
            _strongStunTime = strongStunTime;
        }
        private void OnDisable()
        {
            ResetOnAttack();
        }
        public int GetDemendedCost()
        {
            return _demendedCost;
        }

        public void Attack(int facing)
        {
            var knockback = new Vector2(_force.x * facing, _force.y);
            _pushable.PushTo(knockback);
            _attackable.StartInvincibleTime();

            var enemies = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0, _attackableLayers);

            int totalDmg = _baseDamage;
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
                Managers.Instance.AttackManager.RequestAttack(comp, this, totalDmg, knockback, _baseStunTime + _strongStunTime);
            }

            OnAttackEnd?.Invoke();
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage, transform.position, true);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}