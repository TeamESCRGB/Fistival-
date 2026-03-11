using InputHandler;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HandCoordinator : MonoBehaviour, IRMBInputHandler
    {
        [SerializeField]
        private LayerMask _filter;
        [SerializeField]
        private Transform _anchor;
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

        private Vector2 _mousePos;

        private bool _isGrabbedClick = false;

        public event Action<int,int> OnChargeRateChanged;//now rate, max rate

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

        public void Init(Rigidbody2D parentRb2d)
        {
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            _isGrabbedClick = false;
            _isCharging = false;
            OnChargeRateChanged = null;
        }

        public void Drop()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                _chargeCnt = 0;
                OnChargeRateChanged?.Invoke(_chargeCnt,_chargeMax);
            }        
        }

        private void Throw()
        {
            if(_grabbedObject != null)
            {
                _grabbedObject.Throw((Camera.main.ScreenToWorldPoint(_mousePos) - _anchor.position).normalized,_parentRb2d.linearVelocity,_forceStep*_chargeCnt);
                _chargeCnt = 0;
                _grabbedObject = null;
                OnChargeRateChanged?.Invoke(_chargeCnt, _chargeMax);
            }
        }

        private void Pickup()
        {
            var hit = Physics2D.BoxCast(_anchor.position, new Vector2(2, 2), 0, _anchor.right, 2, _filter);
            if (hit.transform != null && hit.transform.gameObject.TryGetComponent<ObjectCoordinator>(out var comp))
            {
                _grabbedObject = comp;
                comp.PickUp(_anchor);
            }
        }

        public void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed && _grabbedObject == null)
            {
                Pickup();
                if(_grabbedObject != null)
                {
                    _isGrabbedClick = true;
                }
            }
            else if(pressed)
            {
                _chargeTime = 0;
                _chargeCnt = 1;
                _isGrabbedClick = false;
                _isCharging = true;
            }
            else if(_isGrabbedClick == false)
            {
                _isCharging = false;
                _mousePos = screenPos;
                Throw();
            }
        }
    }
}
