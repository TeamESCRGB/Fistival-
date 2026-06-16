using Coordinator.Victims;
using Data;
using Defines;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Hands
{
    public class HandCoordinator : HandCoordinatorBase
    {

        [Header("HandCoordinator Field")]
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        [SerializeField]private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        

        protected Transform _attackBox;
        protected int _baseSmashDamage;
        protected int _strongAttackDamage;
        protected SkillCoordinatorBase _skillBase;

        protected override void OnAwake()
        {
            _attackBox = transform.Find("@AttackBox");
            base.OnAwake();

#if UNITY_EDITOR
            if (_attackBox == null)
            {
                Debug.LogError($"@AttackBox 가 {gameObject.name}의 자식중에 없습니다.");
            }
#endif
            _skillBase = _attackBox.gameObject.GetComponent<SkillCoordinatorBase>();
#if UNITY_EDITOR
            if (_skillBase == null)
            {
                Debug.LogError($"{_attackBox.name} 에 skillbase가 없습니다");

            }
#endif
        }

        public override void UpdateUpdatedData(PlayerData data)
        {
            base.UpdateUpdatedData(data);
            _baseSmashDamage = data.Damage;
            _strongAttackDamage = data.StrongAttackDamage;
        }

        protected override void OnUpdate()
        {
            //게임 일시정지 로직 나중에 추가
            base.OnUpdate();

            if(_attackStatus == AttackStatus.PRESSED)
            {
                if(Time.timeAsDouble - _pressedTime >= _strongRdyThreshold)
                {
                    _attackStatus = AttackStatus.STRONG_RDY;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG_RDY);
                }
            }
        }

        public virtual void Init(Rigidbody2D parentRb2d, int baseSmashDamage,int strongAttackDamage ,LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask,forcePerCharge, chargeTimeInterval,attackCooldwn);
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            ResetEvents();
            _baseSmashDamage = baseSmashDamage;
            _strongAttackDamage = strongAttackDamage;

            _skillBase.Init(_attackableMask,_baseSmashDamage);
        }

        public virtual void Attack()
        {
            //var enemy = Physics2D.OverlapBox(_attackBox.position, _attackBox.localScale, 0, _attackableMask);//gc
            var enemies = Physics2D.OverlapBoxAll(_attackBox.position, _attackBox.localScale, 0, _attackableMask);
            
            if(enemies is null)
            {
                return;
            }

            for(int i = 0; i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    return;
                }

                //BoxOverlap에 필터링에 걸린것만 가져와서 수행.
                //없으면 실행 안함
                int totalDmg = _baseSmashDamage;

                if(_attackStatus == AttackStatus.STRONG)
                {
                    totalDmg += _strongAttackDamage;
                }

                if (_grabbedObject != null)
                {

                    totalDmg += _grabbedObject.GetSharedData().Damage;

                    if (_grabbedObject.Smash() == false)
                    {
                        _grabbedObject = null;
                        _chargeCnt = 0;
                        InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                        InvokeOnGrabbedObjectChanged(null);
                        _status = HandStatus.IDLE;
                    }
                }

                Vector2 knockback = new Vector2(transform.forward.z * totalDmg,0);
                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg, knockback);
            }
        }

        public void StopAttack()
        {
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
        }

        public override void OnLMBPressed()
        {
            if(CanUseWeapon())
            {
                if(_weapon.HasInternalTimer() || _cooldownModule.IsCooldownEnded())
                {
                    _pressedTime = 0;
                    _attackStatus = AttackStatus.WEAPON;
                    _cooldownModule.StopCooldown();
                    base.OnLMBPressed();
                }

                return;
            }


            if(_cooldownModule.IsCooldownEnded() == false)
            {
                return;
            }
            _attackStatus = AttackStatus.PRESSED;
            _pressedTime = Time.timeAsDouble;
            OnAttackStatusChanged?.Invoke(AttackStatus.PRESSED);
        }

        public override void OnLMBReleased()
        {
            if(CanUseWeapon())
            {
                if(_attackStatus == AttackStatus.WEAPON && _weapon.HasInternalTimer() == false)
                {
                    _cooldownModule.StartCooldown();
                }
                _attackStatus = AttackStatus.NO_PRESSED;
                _pressedTime = 0;
                base.OnLMBReleased();
                return;
            }

            if(_cooldownModule.IsCooldownEnded() == false || (_attackStatus & (AttackStatus.NO_PRESSED | AttackStatus.WEAPON)) != 0)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                return;
            }

            if(_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
            }
            Attack();
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }


        public override void Drop()
        {
            base.Drop();
            if(_attackStatus == AttackStatus.WEAPON)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
            }
        }

        protected override void Throw()
        {
            base.Throw();
            if (_attackStatus == AttackStatus.WEAPON)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
            }
        }
    }
}