using Coordinator.Victims;
using Defines;
using Manager;
using UnityEngine;
using Coordinator.Objects;
using static Utils.VectorUtils;

namespace Coordinator.Hands
{
    public class HandCoordinator : HandCoordinatorBase
    {
        protected Transform _attackBox;
        protected int _baseSmashDamage;
        protected SkillCoordinatorBase _skillBase;
        protected override void OnAwake()
        {
            _handAnchor = transform.Find("@HandAnchor");
            _attackBox = transform.Find("@AttackBox");
            _mainCam = Camera.main;

#if UNITY_EDITOR
            if (_handAnchor == null)
            {
                Debug.LogError($"@HandAnchor 가 {gameObject.name}의 자식중에 없습니다.");
            }

            if (_attackBox == null)
            {
                Debug.LogError($"@AttackBox 가 {gameObject.name}의 자식중에 없습니다.");
            }

            if (_mainCam == null)
            {
                Debug.LogError($"MainCamera테그를 가진 카메라가 없습니다.");
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
        }

        

        public virtual void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
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
    }
}