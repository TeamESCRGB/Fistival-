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

        protected override void OnAwake()
        {
            base.OnAwake();
            _chain = transform.Find("@ChainMorningStar")?.GetComponent<ChainMorningStar>();

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
            _chain.SetRotation(GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), transform.position));
        }

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage,int strongAttackDamage ,LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn, int throwAdditionalDamage, float strongAttackThreshold)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn, throwAdditionalDamage);
            ResetEvents();
            _strongAttackThreshold = strongAttackThreshold;
            _strongRdyThreshold = _strongAttackThreshold / 2;
            _chain.Init(attackableFilter,parentRb2d.transform, baseSmashDamage,strongAttackDamage ,GetComponentInParent<IChainPullable>());
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _chain.transform.SetParent(null);
        }

        public void StopAttack()
        {
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

            if (_cooldownModule.IsCooldownEnded() == false)
            {
                return;
            }
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

            if (_cooldownModule.IsCooldownEnded() == false || (_attackStatus & (AttackStatus.NO_PRESSED | AttackStatus.WEAPON)) != 0)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                return;
            }

            if (_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
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