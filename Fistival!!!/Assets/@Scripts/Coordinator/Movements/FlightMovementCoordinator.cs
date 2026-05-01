using InputHandler;
using UnityEngine;

namespace Assets._Scripts.Coordinator.Movements
{
    public class FlightMovementCoordinator :MonoBehaviour, IHorizontalMovementInputHandler, IVerticalMovementInputHandler
    {
        [SerializeField]
        private LayerMask _groundLayer;
        private Rigidbody2D _parentRb2d;
        private float _speed;
        private Vector2 _vel;

        private sbyte _keyStatus = 0;//0번비트:rightOn, 1번비트: leftOn, 2번비트: upOn, 3번비트:downOn
        private const sbyte _rightOnMask = 1 << 0;
        private const sbyte _leftOnMask = 1 << 1;
        private const sbyte _upOnMask = 1 << 2;
        private const sbyte _downOnMask = 1 << 3;

        public virtual void Init(float speed, Rigidbody2D parentRb2d)
        {
            _parentRb2d = parentRb2d;
            _speed = speed;
            _keyStatus = 0;
        }

        private void FixedUpdate()
        {
            if (_parentRb2d == null)
            {
                return;
            }

            _parentRb2d.linearVelocity = _vel;
        }

        public void OnLeftMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= _leftOnMask;
                _vel.x = -_speed;
            }
            else
            {
                _keyStatus &= ~_leftOnMask;
                RestoreHorizontalMovementState();
            }
        }

        public void OnRightMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= _rightOnMask;
                _vel.x = _speed;
            }
            else
            {
                _keyStatus &= ~_rightOnMask;
                RestoreHorizontalMovementState();
            }
        }

        public void OnDownMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= _downOnMask;
                _vel.y = -_speed;
            }
            else
            {
                _keyStatus &= ~_downOnMask;
                RestoreVerticalMovementState();
            }
        }

        public void OnUpMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= _upOnMask;
                _vel.y = _speed;
            }
            else
            {
                _keyStatus &= ~_upOnMask;
                RestoreVerticalMovementState();
            }
        }

        private void RestoreHorizontalMovementState()
        {
            _vel.x = 0;

            if ((_keyStatus & _leftOnMask) == _leftOnMask)
            {
                _vel.x = -_speed;
            }
            else if ((_keyStatus & _rightOnMask) == _rightOnMask)
            {
                _vel.x = _speed;
            }
        }

        private void RestoreVerticalMovementState()
        {
            _vel.y = 0;

            if ((_keyStatus & _downOnMask) == _downOnMask)
            {
                _vel.y = -_speed;
            }
            else if ((_keyStatus & _upOnMask) == _upOnMask)
            {
                _vel.y = _speed;
            }
        }
    }
}
