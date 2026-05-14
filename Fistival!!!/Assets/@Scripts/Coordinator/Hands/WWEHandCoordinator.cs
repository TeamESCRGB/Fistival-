using Coordinator.Victims;
using Defines;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Hands
{
    public class WWEHandCoordinator : HandCoordinatorBase
    {
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        [SerializeField]
        private int _strongDamageMultiplier = 2;
        [SerializeField] private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        private int _baseSmashDamage;

        [SerializeField]
        private float _comboThreshold=0.5f;
        private float _lastComboInput;
        [SerializeField]private WWESkillTypes _skillType;
        public event Action<WWESkillTypes> OnComboChanged;

        protected override void OnUpdate()
        {
            //게임 일시정지 로직 나중에 추가
            base.OnUpdate();

            if (_attackStatus == AttackStatus.PRESSED)
            {
                if (Time.timeAsDouble - _pressedTime >= _strongRdyThreshold)
                {
                    _attackStatus = AttackStatus.STRONG_RDY;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG_RDY);
                }
            }

            if(_skillType != WWESkillTypes.NORMAL)
            {
                _lastComboInput -= Time.deltaTime;
                if (_lastComboInput <= 0)
                {
                    _skillType = WWESkillTypes.NORMAL;
                    OnComboChanged?.Invoke(WWESkillTypes.NORMAL);
                }
            }
        }

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn);
            _attackStatus = AttackStatus.NO_PRESSED;
            _skillType = WWESkillTypes.NORMAL;
            _pressedTime = 0;
            _lastComboInput = 0;
            ResetEvents();
            _baseSmashDamage = baseSmashDamage;
        }

        public int GetStrongAttackDamageMultiplier()
        {
            return _strongDamageMultiplier;
        }
        public void StopAttack()
        {
            _skillType = WWESkillTypes.NORMAL;
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            OnComboChanged?.Invoke(WWESkillTypes.NORMAL);
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
                return;
            }

            if (_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
            }

            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }
    }
}