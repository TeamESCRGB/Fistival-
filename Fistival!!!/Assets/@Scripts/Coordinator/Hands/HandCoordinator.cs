using Coordinator.Victims;
using Data;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Hands
{
    public class HandCoordinator : HandCoordinatorBase
    {

        [Header("HandCoordinator Field")]
        private double _strongRdyThreshold = 0.5f;
        private double _strongAttackThreshold = 1;
        [SerializeField]private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        

        protected Transform _attackBox;
        protected int _baseSmashDamage;
        protected int _strongAttackDamage;
        protected SkillCoordinatorBase _skillBase;

        protected float _strongStun = 0;

        private ParticleSystem _strongAttackCharging;
        private ParticleSystem _strongAttackChargeEnd;


        protected override void OnAwake()
        {
            _attackBox = transform.Find("@AttackBox");
            _strongAttackCharging = gameObject.GetChild<ParticleSystem>("@StrongChargingParticle", true, true);
            _strongAttackChargeEnd = gameObject.GetChild<ParticleSystem>("@StrongChargeEndParticle", true, true);
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

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override void UpdateUpdatedData(PlayerData data)
        {
            base.UpdateUpdatedData(data);
            _baseSmashDamage = data.Damage;
            _strongAttackDamage = data.StrongAttackDamage;
            _strongAttackThreshold = data.StrongAttackThreshold;
            _strongRdyThreshold = _strongAttackThreshold / 2;
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
            else if(_attackStatus == AttackStatus.STRONG_RDY)
            {
                if(Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
                {
                    _attackStatus = AttackStatus.STRONG;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG);
                    _strongAttackChargeEnd.Stop();
                    _strongAttackChargeEnd.Play();
                }
            }
        }

        public virtual void Init(Rigidbody2D parentRb2d, PlayerData playerData)//int baseSmashDamage,int strongAttackDamage ,LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn, int throwAdditionalDamage, float strongAttackThreshold, float stunTime
        {
            InitCommonDatas(parentRb2d, playerData);
            _strongAttackThreshold = playerData.StrongAttackThreshold;
            _strongRdyThreshold = _strongAttackThreshold / 2;
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            ResetEvents();
            _baseSmashDamage = playerData.Damage;
            _strongAttackDamage = playerData.StrongAttackDamage;
            _strongStun = playerData.StrongStunTime;
            _skillBase.Init(_attackableMask,_baseSmashDamage, playerData.StunTime);
            ClearAttackChargingParticles();
        }

        private void ClearAttackChargingParticles()
        {
            _strongAttackCharging.Stop();
            _strongAttackCharging.Clear();
            _strongAttackChargeEnd.Stop();
            _strongAttackChargeEnd.Clear();
        }

        public virtual void Attack()
        {
            //var enemy = Physics2D.OverlapBox(_attackBox.position, _attackBox.localScale, 0, _attackableMask);//gc
            var enemies = Physics2D.OverlapBoxAll(_attackBox.position, _attackBox.localScale, 0, _attackableMask);
            Debug.Log(_attackStatus == AttackStatus.STRONG ? "강공나감!" : "약공나감!");
            if (enemies is null)
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
                float stunTime = _stunTime;

                if(_attackStatus == AttackStatus.STRONG)
                {
                    stunTime += _strongStun;
                    totalDmg += _strongAttackDamage;
                }

                if (_grabbedObject != null)
                {

                    totalDmg += _grabbedObject.GetSharedData().Damage;

                    if (_grabbedObject.Smash(1<<enemy.gameObject.layer) == false)
                    {
                        _grabbedObject = null;
                        _chargeCnt = 0;
                        InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                        InvokeOnGrabbedObjectChanged(null);
                        _status = HandStatus.IDLE;
                    }
                }

                Vector2 knockback = new Vector2(transform.forward.z * totalDmg,0);
                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg, knockback, stunTime);
            }
        }

        public void StopAttack()
        {
            ClearAttackChargingParticles();
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
            else if(_weapon != null)
            {
                RemoveWeapon();
            }


            if(_cooldownModule.IsCooldownEnded() == false)
            {
                return;
            }

            _strongAttackCharging.Stop();
            _strongAttackCharging.Play(false);

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
            else if (_weapon != null)
            {
                RemoveWeapon();
            }

            if (_cooldownModule.IsCooldownEnded() == false || (_attackStatus & (AttackStatus.NO_PRESSED | AttackStatus.WEAPON)) != 0)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                return;
            }

            _strongAttackCharging.Stop();
            _strongAttackCharging.Clear();
            if(_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
                OnAttackStatusChanged?.Invoke(AttackStatus.STRONG);
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
                OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
                ClearAttackChargingParticles();
            }
        }

        protected override void Throw()
        {
            base.Throw();
            if (_attackStatus == AttackStatus.WEAPON)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
                ClearAttackChargingParticles();
            }
        }
    }
}