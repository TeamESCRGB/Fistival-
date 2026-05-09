using Defines;
using InputHandler;
using System;
using UnityEngine;

namespace Coordinator.Hands
{
    public class RootShooterHand : HandCoordinatorBase, IPointerMovementInputHandler
    {
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        [SerializeField]
        private int _strongDamageMultiplier = 2;
        private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;

        private Transform _attackBox;
        private SkillCoordinatorBase _skillBase;
        private int _baseSmashDamage;

        private int _bulletCnt = 0;
        private int _maxBulletCnt = 7;

        protected override void OnAwake()
        {
            base.OnAwake();
            _attackBox = transform.Find("@AttackBox");
            
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

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn);
            ResetEvents();
            _baseSmashDamage = baseSmashDamage;
            _bulletCnt = _maxBulletCnt;

            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _skillBase.Init(_attackableMask, _baseSmashDamage);
        }

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
        }

        public void Reload()
        {

        }

        public override void OnLMBPressed()
        {
            throw new NotImplementedException();
        }

        public override void OnLMBReleased()
        {
            throw new NotImplementedException();
        }

        public void OnPointerMove(Vector2 screenPos)
        {

        }
    }
}
