using Coordinator.Movements;
using Coordinator.Victims;
using InputHandler;
using Manager;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Coordinator.Skills
{
    public class Tatsumakisenpukyaku : SkillCoordinatorBase
    {
        public event Action OnAttackEnd;
        [SerializeField]
        private int _demendedCost = 0;
        [SerializeField]
        private Vector2 _force;
        [SerializeField]private Rigidbody2D _rb2d;
        private IAttackable _attackable;

        [SerializeField]
        private float _attackInterval;
        private float _lastAttackTime;
        private int _remainAttackTick;
        private int _attackCnt=4;

        private float _targetHeight;

        private bool _isAttack = false;

        private float _gravityScale;


        public void Init(int attackableLayers, int damage, Rigidbody2D rb2d, IAttackable attackable, float baseStunTime)
        {
            base.Init(attackableLayers, damage, baseStunTime);
            _rb2d = rb2d;
            _gravityScale = _rb2d.gravityScale;
            _isAttack = false;
            _targetHeight = 0;
            _lastAttackTime = 0;
            _remainAttackTick = _attackCnt;
            _attackable = attackable;
        }

        private void EndAttack()
        {
            _attackable.StartInvincibleTime();
            _isAttack = false;
            _rb2d.gravityScale= _gravityScale;
            OnAttackEnd?.Invoke();
        }

        private void OnDisable()
        {
            ResetOnAttack();
        }

        public int GetDemendedCost()
        {
            return _demendedCost;
        }

        public void Attack()
        {
            _isAttack = true;
            _attackable.StartInvincibleTime();
            _targetHeight = _rb2d.position.y + math.abs(_rb2d.transform.lossyScale.y);
            _rb2d.AddForce(_force,ForceMode2D.Impulse);
            _remainAttackTick = _attackCnt;
            _lastAttackTime = Time.time;
        }

        private void FixedUpdate()
        {
            if(_isAttack == false)
            {
                return;
            }

            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }

            if(_rb2d.position.y >= _targetHeight)
            {
                _rb2d.gravityScale=0;
                _rb2d.linearVelocityY = 0;
            }

            if (Time.time >= _lastAttackTime + _attackInterval)
            {
                _lastAttackTime+= _attackInterval;
                _remainAttackTick--;


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
                    Managers.Instance.AttackManager.RequestAttack(comp, this, totalDmg, Vector2.zero, _baseStunTime);
                }

                if (_remainAttackTick<=0)
                {
                    EndAttack();
                }
            }
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}