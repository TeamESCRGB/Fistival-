using Coordinator.Victims;
using Data;
using Manager;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HandCoordinator : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _pickupBoxcastSize;
        [SerializeField]
        private float _pickupBoxcastDistance;
        [SerializeField]
        private LayerMask _pickableObjectMask;
        private Transform _handAnchor;
        private Transform _attackBox;
        private ObjectCoordinator _grabbedObject;
        private Rigidbody2D _parentRb2d;

        private int _maxChargeCnt = 3;
        private int _chargeCnt = 0;

        [SerializeField]
        private float _forcePerCharge=10;
        [SerializeField]
        private float _chargeTimeInterval=1;
        private float _chargeTime=0;

        private bool _isCharging = false;

        public Vector2 MousePos { get; set; }

        private bool _isGrabbedClick = false;

        public event Action<int,int> OnChargeRateChanged;//now rate, max rate
        public event Action<ObjectData> OnGrabbedObjectChanged;

        private int _baseSmashDamage;

        private LayerMask _attackableMask=0;

        private SkillCoordinatorBase _skillBase;

        private void Awake()
        {
            _handAnchor = transform.Find("@HandAnchor");
            _attackBox = transform.Find("@AttackBox");

#if UNITY_EDITOR
            if (_handAnchor == null)
            {
                Debug.LogError($"@HandAnchor 가 {gameObject.name}의 자식중에 없습니다.");
            }

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

        private void Update()
        {
            //게임 일시정지 로직 나중에 추가
            if(_isCharging && _chargeCnt < _maxChargeCnt)
            {
                _chargeTime += Time.deltaTime;

                if(_chargeTime >= _chargeTimeInterval)
                {
                    _chargeTime = 0;
                    _chargeCnt += 1;
                    OnChargeRateChanged?.Invoke(_chargeCnt,_maxChargeCnt);
                }
            }
        }

        public int GetMaxCharge()
        {
            return _maxChargeCnt;
        }

        public void SetMaxCharge(int maxCharge)
        {
            if(maxCharge < 0)
            {
                maxCharge = 0;
            }
            _maxChargeCnt = maxCharge;
        }

        public void Attack()
        {
            var enemy = Physics2D.OverlapBox(_attackBox.position, _attackBox.localScale, 0, _attackableMask);

            if(enemy == null || enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            //BoxOverlap에 필터링에 걸린것만 가져와서 수행.
            //없으면 실행 안함
            int totalDmg = _baseSmashDamage;
            if(_grabbedObject != null)
            {

                totalDmg += _grabbedObject.GetSharedData().Damage;

                if(_grabbedObject.Smash() == false)
                {
                    _grabbedObject = null;
                    _chargeCnt = 0;
                    OnChargeRateChanged?.Invoke(_chargeCnt, _maxChargeCnt);
                    OnGrabbedObjectChanged?.Invoke(null);
                }
            }

            Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg);
            //공격하는 함수. 데미지: damage +  해서 OverlapBox써서 나중에 함


        }

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
            _attackableMask = attackableFilter;
            _grabbedObject = null;
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            _isGrabbedClick = false;
            _isCharging = false;
            OnChargeRateChanged = null;
            OnGrabbedObjectChanged = null;
            _baseSmashDamage = baseSmashDamage;

            _skillBase.Init(_attackableMask,_baseSmashDamage);
        }

        public void Drop()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                _chargeCnt = 0;
                OnChargeRateChanged?.Invoke(_chargeCnt,_maxChargeCnt);
                OnGrabbedObjectChanged?.Invoke(null);
            }        
        }

        private void Throw()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.SetAttackableLayer(_attackableMask);
                _grabbedObject.Throw((Camera.main.ScreenToWorldPoint(MousePos) - _handAnchor.position).normalized,_parentRb2d.linearVelocity,_forcePerCharge*_chargeCnt);
                _chargeCnt = 0;
                _grabbedObject = null;
                OnChargeRateChanged?.Invoke(_chargeCnt, _maxChargeCnt);
                OnGrabbedObjectChanged?.Invoke(null);
                
            }
        }

        private void Pickup()
        {
            var hit = Physics2D.BoxCast(_handAnchor.position, _pickupBoxcastSize, 0, _handAnchor.right, _pickupBoxcastDistance, _pickableObjectMask);
            if (hit.transform != null && hit.transform.gameObject.TryGetComponent<ObjectCoordinator>(out var comp))
            {
                _grabbedObject = comp;
                comp.PickUp(_handAnchor);
                OnGrabbedObjectChanged?.Invoke(comp.GetSharedData());
            }
        }

        public void OnRMBPressed()
        {
            if (_grabbedObject == null)
            {
                Pickup();
                if (_grabbedObject != null)
                {
                    _isGrabbedClick = true;
                }
            }
            else
            {
                _chargeTime = 0;
                _chargeCnt = 1;
                _isGrabbedClick = false;
                _isCharging = true;
            }
        }

        public void OnRMBReleased()
        {
            if(_grabbedObject != null && _isGrabbedClick == false)
            {
                _isCharging = false;
                Throw();
            }
        }
    }
}
