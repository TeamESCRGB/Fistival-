using Coordinator.Victims;
using Defines;
using Manager;
using UnityEngine;
using Coordinator.Objects;
using static Utils.VectorUtils;
using System;

namespace Coordinator.Hands
{
    public class HandCoordinator : HandCoordinatorBase
    {

        [Header("HandCoordinator Field")]
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        [SerializeField]
        private int _strongDamageMultiplier = 2;
        private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        

        protected Transform _attackBox;
        protected int _baseSmashDamage;
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



        protected override void OnUpdate()
        {
            //게임 일시정지 로직 나중에 추가
            if(_status == HandStatus.CHARGE && _chargeCnt < _maxChargeCnt)
            {
                _chargeTime += Time.deltaTime;

                if(_chargeTime >= _chargeTimeInterval)
                {
                    _chargeTime = 0;
                    _chargeCnt += 1;
                    InvokeOnChargeRateChanged(_chargeCnt,_maxChargeCnt);
                }
            }

            if(_attackStatus == AttackStatus.PRESSED)
            {
                if(Time.timeAsDouble - _pressedTime >= _strongRdyThreshold)
                {
                    _attackStatus = AttackStatus.STRONG_RDY;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG_RDY);
                }
            }
        }

        

        public virtual void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
            _attackStatus = AttackStatus.NO_PRESSED;
            _pressedTime = 0;
            _status = HandStatus.IDLE;
            _attackableMask = attackableFilter;
            _grabbedObject = null;
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            ResetEvents();
            _baseSmashDamage = baseSmashDamage;

            _skillBase.Init(_attackableMask,_baseSmashDamage);
        }

        public int GetStrongAttackDamageMultiplier()
        {
            return _strongDamageMultiplier;
        }
        public override void Drop()
        {
            if(_grabbedObject != null)
            {
                _status = HandStatus.IDLE;
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                _chargeCnt = 0;
                InvokeOnChargeRateChanged(_chargeCnt,_maxChargeCnt);
                InvokeOnGrabbedObjectChanged(null);
            }        
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
                    totalDmg *= _strongDamageMultiplier;
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

                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg);
            }

            
            //공격하는 함수. 데미지: damage +  해서 OverlapBox써서 나중에 함
        }

        protected override void Throw()
        {
            _status = HandStatus.IDLE;
            _grabbedObject.SetAttackableLayer(_attackableMask);
            _grabbedObject.Throw(GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), _handAnchor.position), _parentRb2d.linearVelocity, _forcePerCharge * _chargeCnt);
            _chargeCnt = 0;
            _grabbedObject = null;
            InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
            InvokeOnGrabbedObjectChanged(null);
        }

        protected override void Pickup()
        {
            _status = HandStatus.IDLE;
            var hit = Physics2D.BoxCast(_handAnchor.position, _pickupBoxcastSize, 0, _handAnchor.right, _pickupBoxcastDistance, _pickableObjectMask);
            if (hit.transform != null && hit.transform.gameObject.TryGetComponent<ObjectCoordinator>(out var comp))
            {
                _grabbedObject = comp;
                comp.PickUp(_handAnchor);
                InvokeOnGrabbedObjectChanged(comp.GetSharedData());
                _status = HandStatus.GRABBED;
            }
        }

        public override void OnRMBPressed()
        {
            if (_grabbedObject == null)
            {
                Pickup();
            }
            else
            {
                _chargeTime = 0;
                _chargeCnt = 1;
                _status = HandStatus.CHARGE;
            }
        }

        public override void OnRMBReleased()
        {
            if(_status == HandStatus.CHARGE && _grabbedObject != null)
            {
                Throw();
            }
        }

        public override void OnLMBPressed()
        {
            _attackStatus = AttackStatus.PRESSED;
            _pressedTime = Time.timeAsDouble;
            OnAttackStatusChanged?.Invoke(AttackStatus.PRESSED);
        }

        public override void OnLMBReleased()
        {

            if(_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
            }
            Attack();
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
        }
    }
}