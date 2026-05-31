using ComponentModule;
using Defines;
using InputHandler;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Movements
{
    public class FlightMovementCoordinator :MonoBehaviour, IHorizontalMovementInputHandler, IVerticalMovementInputHandler, IPushable, IMovementLockable
    {
        [SerializeField]
        private LayerMask _groundLayer;
        private Rigidbody2D _parentRb2d;
        private float _speed;
        private Directions _nextXDir;
        private Directions _nextYDir;
        private MovementState _xMovState;
        private MovementState _yMovState;


        private MovementKeyStatus _keyStatus = MovementKeyStatus.OFF;//0번비트:rightOn, 1번비트: leftOn, 2번비트: upOn, 3번비트:downOn
        private const MovementKeyStatus _verticalMask = MovementKeyStatus.UP | MovementKeyStatus.DOWN;
        private const MovementKeyStatus _horizontalMask = MovementKeyStatus.LEFT | MovementKeyStatus.RIGHT;

        private bool _isInputLocked;
        private int _lockCnt;

        public virtual void Init(float speed, Rigidbody2D parentRb2d)
        {
            _lockCnt = 0;
            _parentRb2d = parentRb2d;
            _speed = speed;
            _keyStatus = MovementKeyStatus.OFF;
            _nextYDir = _nextXDir = Directions.OFF;
            _yMovState = _xMovState = MovementState.OFF;
            _isInputLocked = false;
        }

        private void FixedUpdate()
        {
            if (Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            if (_parentRb2d == null)
            {
                return;
            }

            if(_isInputLocked == false)
            {
                _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed, (float)_nextXDir, ref _xMovState);
                _parentRb2d.linearVelocityY = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityY, _speed, (float)_nextYDir, ref _yMovState);   
            }
        }

        public void LockMovement()
        {
            _lockCnt++;
            if(_isInputLocked)
            {
                return;
            }

            if (_xMovState != MovementState.START_REQ && _xMovState != MovementState.STOP_ACCEPT)
            {
                MovementState stopReq = MovementState.STOP_REQ;
                _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed, 0, ref stopReq);
            }

            if (_yMovState != MovementState.START_REQ && _yMovState != MovementState.STOP_ACCEPT)
            {
                MovementState stopReq = MovementState.STOP_REQ;
                _parentRb2d.linearVelocityY = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityY, _speed, 0, ref stopReq);
            }

            _isInputLocked = true;
        }

        public void UnlockMovement()
        {
            if(_isInputLocked == false)
            {
                return;
            }

            _lockCnt--;
            if(_lockCnt > 0)
            {
                return;
            }

            _isInputLocked = false;

            if (_xMovState == MovementState.STOP_REQ)
            {
                _xMovState = MovementState.STOP_ACCEPT;
            }

            if (_yMovState == MovementState.STOP_REQ)
            {
                _yMovState = MovementState.STOP_ACCEPT;
            }

            _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed, (float)_nextXDir, ref _xMovState);
            _parentRb2d.linearVelocityY = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityY, _speed, (float)_nextYDir, ref _yMovState);
        }


        public void PushTo(Vector2 force)
        {
            _parentRb2d.AddForce(force, ForceMode2D.Impulse);
        }

        public void OnLeftMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                if((_keyStatus & _horizontalMask) == MovementKeyStatus.OFF)
                {
                    _xMovState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.LEFT;
                _nextXDir = Directions.LEFT;
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
                if ((_keyStatus & _horizontalMask) == MovementKeyStatus.OFF)
                {
                    _xMovState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.RIGHT;
                _nextXDir = Directions.RIGHT;
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
                if ((_keyStatus & _verticalMask) == MovementKeyStatus.OFF)
                {
                    _yMovState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.DOWN;
                _nextYDir = Directions.DOWN;
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
                if ((_keyStatus & _verticalMask) == MovementKeyStatus.OFF)
                {
                    _yMovState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.UP;
                _nextYDir = Directions.UP;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.UP;
                RestoreVerticalMovementState();
            }
        }

        private void RestoreHorizontalMovementState()
        {
            _nextXDir = Directions.OFF;

            if ((_keyStatus & MovementKeyStatus.LEFT) == MovementKeyStatus.LEFT)
            {
                _nextXDir = Directions.LEFT;
            }
            else if ((_keyStatus & MovementKeyStatus.RIGHT) == MovementKeyStatus.RIGHT)
            {
                _nextXDir = Directions.RIGHT;
            }
            else if ((_keyStatus & _horizontalMask) == MovementKeyStatus.OFF)
            {
                _xMovState = MovementState.STOP_REQ;
            }
        }

        private void RestoreVerticalMovementState()
        {
            _nextYDir = Directions.OFF;

            if ((_keyStatus & MovementKeyStatus.DOWN) == MovementKeyStatus.DOWN)
            {
                _nextYDir = Directions.DOWN;
            }
            else if ((_keyStatus & MovementKeyStatus.UP) == MovementKeyStatus.UP)
            {
                _nextYDir = Directions.UP;
            }
            else if ((_keyStatus & _verticalMask) == MovementKeyStatus.OFF)
            {
                _yMovState = MovementState.STOP_REQ;
            }
        }
    }
}