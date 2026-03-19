using Data;
using InputHandler;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HandCoordinator : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _reachBox;
        [SerializeField]
        private float _reachDistance;
        [SerializeField]
        private LayerMask _objectFilter;
        [SerializeField]
        private Transform _handAnchor;
        [SerializeField]
        private Transform _attackPivot;
        private ObjectCoordinator _grabbedObject;
        private Rigidbody2D _parentRb2d;

        private int _chargeMax = 3;
        private int _chargeCnt = 0;

        [SerializeField]
        private float _forceStep=10;
        [SerializeField]
        private float _chargeTimeInterval=1;
        private float _chargeTime=0;

        private bool _isCharging = false;

        public Vector2 MousePos { get; set; }

        private bool _isGrabbedClick = false;

        public event Action<int,int> OnChargeRateChanged;//now rate, max rate
        public event Action<ObjectData> OnGrabbedObjectChanged;

        private int _damage;

        private LayerMask _attackableFilter=0;
        private Vector2 _attackBoxSize;

        private void Update()
        {
            //게임 일시정지 로직 나중에 추가
            if(_isCharging && _chargeCnt < _chargeMax)
            {
                _chargeTime += Time.deltaTime;

                if(_chargeTime >= _chargeTimeInterval)
                {
                    _chargeTime = 0;
                    _chargeCnt += 1;
                    OnChargeRateChanged?.Invoke(_chargeCnt,_chargeMax);
                }
            }
        }

        public int GetMaxCharge()
        {
            return _chargeMax;
        }

        public void SetMaxCharge(int maxCharge)
        {
            if(maxCharge < 0)
            {
                maxCharge = 0;
            }
        }

        public void Attack()
        {
            var enemy = Physics2D.OverlapBox(_attackPivot.position, _attackBoxSize, 0, _attackableFilter);
            //BoxOverlap에 필터링에 걸린것만 가져와서 수행.
            //없으면 실행 안함
            int totalDmg = _damage;
            if(_grabbedObject != null)
            {

                totalDmg += _grabbedObject.GetSharedData().Damage;
                if(_grabbedObject.Smash() == false)
                {
                    _grabbedObject = null;
                    _chargeCnt = 0;
                    OnChargeRateChanged?.Invoke(_chargeCnt, _chargeMax);
                    OnGrabbedObjectChanged?.Invoke(null);
                }
            }
            //공격하는 함수. 데미지: damage +  해서 OverlapBox써서 나중에 함
            

        }

        public void Init(Rigidbody2D parentRb2d, int damage, LayerMask attackableFilter)
        {
            _attackBoxSize = _attackPivot.localScale;
            _attackableFilter = attackableFilter;
            _grabbedObject = null;
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            _isGrabbedClick = false;
            _isCharging = false;
            OnChargeRateChanged = null;
            OnGrabbedObjectChanged = null;
            _damage = damage;
        }

        public void Drop()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                _chargeCnt = 0;
                OnChargeRateChanged?.Invoke(_chargeCnt,_chargeMax);
                OnGrabbedObjectChanged?.Invoke(null);
            }        
        }

        private void Throw()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.SetAttackableLayer(_attackableFilter);
                _grabbedObject.Throw((Camera.main.ScreenToWorldPoint(MousePos) - _handAnchor.position).normalized,_parentRb2d.linearVelocity,_forceStep*_chargeCnt);
                _chargeCnt = 0;
                _grabbedObject = null;
                OnChargeRateChanged?.Invoke(_chargeCnt, _chargeMax);
                OnGrabbedObjectChanged?.Invoke(null);
                
            }
        }

        private void Pickup()
        {
            var hit = Physics2D.BoxCast(_handAnchor.position, _reachBox, 0, _handAnchor.right, _reachDistance, _objectFilter);
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
