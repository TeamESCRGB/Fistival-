using Defines;
using InputHandler;
using UnityEngine;

namespace Coordinator.Movements
{
    public class FlightMovementCoordinator :MonoBehaviour, IHorizontalMovementInputHandler, IVerticalMovementInputHandler
    {
        [SerializeField]
        private LayerMask _groundLayer;
        private Rigidbody2D _parentRb2d;
        private float _speed;
        private Vector2 _vel;

        private MovementKeyStatus _keyStatus = MovementKeyStatus.OFF;//0번비트:rightOn, 1번비트: leftOn, 2번비트: upOn, 3번비트:downOn
        public virtual void Init(float speed, Rigidbody2D parentRb2d)
        {
            _parentRb2d = parentRb2d;
            _speed = speed;
            _keyStatus = MovementKeyStatus.OFF;
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
                _keyStatus |= MovementKeyStatus.LEFT;
                _vel.x = -_speed;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.LEFT;
                RestoreHorizontalMovementState();
            }
        }

        public void OnRightMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= MovementKeyStatus.RIGHT;
                _vel.x = _speed;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.RIGHT;
                RestoreHorizontalMovementState();
            }
        }

        public void OnDownMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= MovementKeyStatus.DOWN;
                _vel.y = -_speed;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.DOWN;
                RestoreVerticalMovementState();
            }
        }

        public void OnUpMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _keyStatus |= MovementKeyStatus.UP;
                _vel.y = _speed;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.UP;
                RestoreVerticalMovementState();
            }
        }

        private void RestoreHorizontalMovementState()
        {
            _vel.x = 0;

            if ((_keyStatus & MovementKeyStatus.LEFT) == MovementKeyStatus.LEFT)
            {
                _vel.x = -_speed;
            }
            else if ((_keyStatus & MovementKeyStatus.RIGHT) == MovementKeyStatus.RIGHT)
            {
                _vel.x = _speed;
            }
        }

        private void RestoreVerticalMovementState()
        {
            _vel.y = 0;

            if ((_keyStatus & MovementKeyStatus.DOWN) == MovementKeyStatus.DOWN)
            {
                _vel.y = -_speed;
            }
            else if ((_keyStatus & MovementKeyStatus.UP) == MovementKeyStatus.UP)
            {
                _vel.y = _speed;
            }
        }
    }
}
