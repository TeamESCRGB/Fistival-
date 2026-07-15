using Coordinator.Chain;
using Coordinator.Movements;
using Data;
using Defines;
using InputHandler;
using System;
using UnityEngine;
using Utils;
using static Utils.VectorUtils;

namespace Coordinator.Hands
{
    public class MetroidvaniaHand : HandCoordinatorBase, IPointerMovementInputHandler
    {
        private double _strongRdyThreshold = 0.5f;
        private double _strongAttackThreshold = 1;
        [SerializeField] private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;

        private ChainMorningStar _chain;

        private ParticleSystem _strongAttackCharging;
        private ParticleSystem _strongAttackChargeEnd;

        private void ClearAttackChargingParticles()
        {
            _strongAttackCharging.Stop();
            _strongAttackCharging.Clear();
            _strongAttackChargeEnd.Stop();
            _strongAttackChargeEnd.Clear();
        }


        protected override void OnAwake()
        {
            base.OnAwake();
            _chain = transform.Find("@ChainMorningStar")?.GetComponent<ChainMorningStar>();
            _strongAttackCharging = gameObject.GetChild<ParticleSystem>("@StrongChargingParticle", true, true);
            _strongAttackChargeEnd = gameObject.GetChild<ParticleSystem>("@StrongChargeEndParticle", true, true);

#if UNITY_EDITOR
            Debug.Assert(_chain != null, "@ChainMorningStar가 없거나 거기에 ChainMorningStar가 없습니다");
#endif
        }

        public void DeInit()
        {
            _chain.Retrive();
            _chain.transform.SetParent(transform);
        }

        public override void UpdateUpdatedData(PlayerData data)
        {
            base.UpdateUpdatedData(data);
            _chain.SetDamage(data.Damage);
            _chain.SetStrongDamage(data.StrongAttackDamage);
            _strongAttackThreshold = data.StrongAttackThreshold;
            _strongRdyThreshold = _strongAttackThreshold / 2;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_attackStatus == AttackStatus.PRESSED)
            {
                if (Time.timeAsDouble - _pressedTime >= _strongRdyThreshold)
                {
                    _attackStatus = AttackStatus.STRONG_RDY;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG_RDY);
                }
            }
            else if (_attackStatus == AttackStatus.STRONG_RDY)
            {
                if (Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
                {
                    _attackStatus = AttackStatus.STRONG;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG);
                    _strongAttackChargeEnd.Stop();
                    _strongAttackChargeEnd.Play();
                }
            }
            _chain.SetRotation(GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), transform.position));
        }

        public void Init(Rigidbody2D parentRb2d, PlayerData playerData)//int baseSmashDamage,int strongAttackDamage ,LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn, int throwAdditionalDamage, float strongAttackThreshold, float stunTime
        {
            InitCommonDatas(parentRb2d, playerData);
            ResetEvents();
            _strongAttackThreshold = playerData.StrongAttackThreshold;
            _strongRdyThreshold = _strongAttackThreshold / 2;
            _chain.Init(playerData.AttackableLayers,parentRb2d.transform, playerData.Damage,playerData.StrongAttackDamage ,GetComponentInParent<IChainPullable>(), playerData.StunTime);
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _chain.transform.SetParent(null);
            ClearAttackChargingParticles();
        }

        public void StopAttack()
        {
            ClearAttackChargingParticles();
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
        }

        public override void OnLMBPressed()
        {
            if (CanUseWeapon())
            {
                if (_weapon.HasInternalTimer() || _cooldownModule.IsCooldownEnded())
                {
                    _pressedTime = 0;
                    _attackStatus = AttackStatus.WEAPON;
                    _cooldownModule.StopCooldown();
                    base.OnLMBPressed();
                }

                return;
            }
            else if (_weapon != null)
            {
                RemoveWeapon();
            }

            if (_cooldownModule.IsCooldownEnded() == false)
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
            if (CanUseWeapon())
            {
                if (_attackStatus == AttackStatus.WEAPON && _weapon.HasInternalTimer() == false)
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
            if (_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
                OnAttackStatusChanged?.Invoke(AttackStatus.STRONG);
            }

            _chain.Launch(GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), transform.position),_attackStatus);

            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }

        public void OnPointerMove(Vector2 screenPos)
        {
            SetMousePos(screenPos);
        }
        public override void Drop()
        {
            base.Drop();
            if (_attackStatus == AttackStatus.WEAPON)
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