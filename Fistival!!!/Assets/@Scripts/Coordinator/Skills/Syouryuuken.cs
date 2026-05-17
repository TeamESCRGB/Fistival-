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

        public void Init(int attackableLayers, int damage, IPushable pushable, IAttackable attackable)
        {
            base.Init(attackableLayers, damage);
            _pushable = pushable;
            _attackable = attackable;
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

            var enemies = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, _attackableLayers);

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
                Managers.Instance.AttackManager.RequestAttack(comp, this, totalDmg, knockback);
            }

            OnAttackEnd?.Invoke();
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}