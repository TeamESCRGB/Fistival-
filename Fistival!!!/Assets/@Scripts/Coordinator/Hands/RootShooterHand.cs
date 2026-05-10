using ComponentModule;
using Coordinator.Victims;
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
        public event Action OnReload;
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
            _attackBox.SetParent(null,false);//나중에 ui로 옮기면 바꾸고, 옮기면 그대로.
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

            _reloadCooldown.OnCooldownEnded += (() => OnReload?.Invoke());//나중에 gc상태 보고 따로 뺴두든지 한다
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
            var enemies = Physics2D.OverlapCircleAll(_attackBox.position, _attackBox.localScale.x / 2, _attackableMask);

            if (enemies is null)
            {
                return;
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    return;
                }

                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, _baseSmashDamage);
            }
        }

        public void Reload()
        {
            /*
             재장전이 불가능한 경우(스턴으로 인한건 Mode에서 막아주니까 제외하고)
             이미 만발일 때
             재장전 쿨타임이 다 안됐을 때
             공격 후 재장전 쿨타임이 다 안됐을 때
             총 상태가 Fanning일 때
            ---
            재장전을 하면:
            재장전 쿨타임을 활성화한다
            총 상태를 RELOAD로 바꾼다
            끝나면 공격 쿨타임을 Stop을 한다

            ---

            지금은 Hand단위에서 거르고,
            나중에 애니메이션 타임에 따라서 해야된다 하면, 작동 할 수 있는지 검사하는 로직을 만들어서 위에서 확인하도록 한다

            ---

            근데, 저거 RELOAD상태는 굳이 있을 필요가 있나
            어차피 쿨타임 체크하면 될텐데
            */
            if(_bulletCnt >= _maxBulletCnt || _gunStatus == GunStatus.FANNING || _reloadUnlockCounter.IsCooldownEnded() == false || _reloadCooldown.IsCooldownEnded() == false)
            {
                return;
            }
            _bulletCnt = _maxBulletCnt;
            _reloadCooldown.StartCooldown();
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
        }

        public void OnPointerMove(Vector2 screenPos)
        {
            //조준선은 ui업데이트 할 때 좌클릭 눌렀을때만 나오게 한다.
            SetMousePos(screenPos);
            Vector3 wp = _mainCam.ScreenToWorldPoint(screenPos);
            wp.z = _attackBox.position.z;
            _attackBox.position = wp;
        }


        public override void OnRMBPressed()
        {
            if ((_gunStatus == GunStatus.FANNING || _reloadCooldown.IsCooldownEnded() == false) && _grabbedObject == null)
            {
                return;
            }
            base.OnRMBPressed();
        }

        public override void OnRMBReleased()
        {
            base.OnRMBReleased();
        }


        #region Callbacks

        #endregion
    }
}