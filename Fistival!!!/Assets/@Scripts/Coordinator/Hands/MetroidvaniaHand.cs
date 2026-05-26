using Coordinator.Chain;
using Coordinator.Movements;
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
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
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
            _chain.transform.SetParent(transform);
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

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn);
            ResetEvents();
            _chain.Init(attackableFilter,parentRb2d, baseSmashDamage, GetComponentInParent<IChainPullable>());
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _chain.transform.SetParent(null);
        }

        public void StopAttack()
        {
            _chain.Retrive();
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
        }

        public override void OnLMBPressed()
        {
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
            if (_cooldownModule.IsCooldownEnded() == false || _attackStatus == AttackStatus.NO_PRESSED)
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
    }
}