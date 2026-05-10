using ComponentModule;
using Defines;
using InputHandler;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.Hands
{
    public class RootShooterHand : HandCoordinatorBase, IPointerMovementInputHandler
    {
        private GunStatus _gunStatus;

        #region 강공_상태_조정
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        #endregion

        #region 공격_박스_설정
        private Transform _attackBox;
        private SkillCoordinatorBase _skillBase;
        private int _baseSmashDamage;
        #endregion

        #region 패닝샷
        [SerializeField]
        private float _fanningInterval = 0.1f;
        private float _lastShootTime = 0;
        #endregion

        #region 장전
        [SerializeField]
        private float _reloadTime;
        private int _bulletCnt = 0;
        private int _maxBulletCnt = 7;
        #endregion

        #region 입력락_(임시분류)
        //쿨타임 락
        private CooldownComponentModule _reloadCooldown;//장전동안 다른 행동 막는 락
        private CooldownComponentModule _reloadUnlockCounter;//공격 후 재장전 막는 락
        private bool _isAttack;//공격동안 다른 입력 막는 락
        #endregion

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

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if(_reloadCooldown is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_reloadCooldown);
                _reloadCooldown = null;
            }

            if(_reloadUnlockCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_reloadUnlockCounter);
                _reloadUnlockCounter = null;
            }
        }

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn);
            ResetEvents();
            _baseSmashDamage = baseSmashDamage;
            _bulletCnt = _maxBulletCnt;

            _gunStatus = GunStatus.USE;
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _skillBase.Init(_attackableMask, _baseSmashDamage);
            if(_reloadCooldown is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_reloadCooldown);
                _reloadCooldown = null;
            }
            if (_reloadUnlockCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_reloadUnlockCounter);
                _reloadUnlockCounter = null;
            }
            _reloadCooldown = Managers.Instance.CooldownManager.GetCooldownModule(_reloadTime);
            _reloadUnlockCounter = Managers.Instance.CooldownManager.GetCooldownModule(attackCooldwn/2);
            _isAttack = false;
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

        public void StopAttack()
        {

        }

        private void Attack() //이건 순수히 공격만 하고 패닝/단일샷 이거는 호출부에서 생각
        {

        }
        
        public void Reload()
        {

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
            Attack();
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }

        public void OnPointerMove(Vector2 screenPos)
        {
            //조준선은 ui업데이트 할 때 좌클릭 눌렀을때만 나오게 한다.
            SetMousePos(screenPos);
            Vector3 wp = _mainCam.ScreenToWorldPoint(screenPos);
            wp.z = _attackBox.position.z;
            _attackBox.position = wp;
        }
    }
}