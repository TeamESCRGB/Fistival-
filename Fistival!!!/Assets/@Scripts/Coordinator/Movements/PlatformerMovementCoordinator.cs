using ComponentModule;
using Defines;
using InputHandler;
using Manager;
using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Coordinator.Movements
{
    public class PlatformerMovementCoordinator : MonoBehaviour ,IJumpsMovementInputHandler, IHorizontalMovementInputHandler, IPushable
    {
        [SerializeField]
        private float _platformIgnoreTime = 0.5f;
        
        [SerializeField]
        private LayerMask _platformMask;
        [SerializeField]
        private LayerMask _groundLayer;

        private Transform _groundedCheckBox;
        

        private Collider2D _parentCol;
        private Rigidbody2D _parentRb2d;
        private Transform _parentTransform;
        private float _speed;
        private float _jumpPow;
        private float _slowness;

        private float _slownessSensitivity=1;
        private float _maxSlowness=0;

        private Vector3 _leftRotation = new Vector3(0, 180, 0);
        private WaitForSeconds _platformEnableDelay;

        private MovementKeyStatus _keyStatus = MovementKeyStatus.OFF;
        private Directions _nextDir;
        [SerializeField]private MovementState _movState;
        
        [SerializeField]
        private float _coyoteTime = 0.1f;
        protected float _coyoteTimeCounter = -1;

        [SerializeField]
        protected float _jumpBufferTime = 0.12f;
        protected float _jumpBufferCounter = -1;

        private bool _isGrounded = false;

        private void Awake()
        {
            _platformEnableDelay = new WaitForSeconds(_platformIgnoreTime);
            _groundedCheckBox = transform.Find("@GroundedCheckBox");
        }

        public virtual void Init(float speed,float jumpPow ,float slownessSensitivity,float maxSlowness,Rigidbody2D parentRb2d)
        {
            _movState = MovementState.OFF;
            _nextDir = Directions.OFF;
            _parentRb2d = parentRb2d;
            _speed = speed;
            _jumpPow = jumpPow;
            _parentCol = _parentRb2d.gameObject.GetComponent<Collider2D>();
            _parentTransform = _parentRb2d.transform;
            _slowness = 1;
            _maxSlowness = maxSlowness;
            _slownessSensitivity = slownessSensitivity;
            _keyStatus = MovementKeyStatus.OFF;
            _jumpBufferCounter = -1;
            _coyoteTimeCounter = -1;
        }

        private void FixedUpdate()
        {
            _isGrounded = IsGrounded();
            if (_isGrounded)
            {
                _coyoteTimeCounter = _coyoteTime;
            }
            else
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }

            _jumpBufferCounter -= Time.fixedDeltaTime;

            if(_isGrounded && _jumpBufferCounter>=0)
            {
                Jump();
            }

            if(_parentRb2d == null)
            {
                return;
            }

            _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed * _slowness, (float)_nextDir ,ref _movState);
        }

        public void PushTo(Vector2 force)
        {
            _parentRb2d.AddForce(force, ForceMode2D.Impulse);
        }

        public void SetSlowness(float slowness)
        {
            if(_slownessSensitivity == 0)
            {
                _slowness = 1;
                return;
            }

            _slowness = Mathf.Clamp(slowness / _slownessSensitivity, _maxSlowness, 1);
        }

        public void OnDownJumpMovementInputEvent(bool pressed)
        {
            if(pressed == false || _parentRb2d == null || _parentCol == null)
            {
                return;
            }

            var col = Physics2D.OverlapBox(_groundedCheckBox.position, _groundedCheckBox.localScale, 0, _platformMask);
            if (col != null)
            {
                StartCoroutine(DisablePlatform(col));
            }
        }

        private IEnumerator DisablePlatform(Collider2D col)
        {
            Physics2D.IgnoreCollision(col,_parentCol);
            yield return _platformEnableDelay ;
            Physics2D.IgnoreCollision(col, _parentCol, false);
        }

        protected void Jump()
        {
            _jumpBufferCounter = -1;
            _coyoteTimeCounter = -1;
            _parentRb2d.linearVelocityY = 0;
            _parentRb2d.AddForce(Vector2.up * _jumpPow, ForceMode2D.Impulse);
        }

        public virtual void OnJumpMovementInputEvent(bool pressed)
        {
            if(pressed == false)
            {
                return;
            }
            if (IsGrounded() || _coyoteTimeCounter >= 0)
            {
                Jump();
            }
            else
            {
                _jumpBufferCounter = _jumpBufferTime;
            }
        }

        public void OnLeftMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                if(_keyStatus == MovementKeyStatus.OFF)
                {
                    _movState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.LEFT;
                _nextDir = Directions.LEFT;
                _parentTransform.eulerAngles = _leftRotation;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.LEFT;
                RestoreMovementState();
            }
        }

        public void OnRightMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                if (_keyStatus == MovementKeyStatus.OFF)
                {
                    _movState = MovementState.START_REQ;
                }
                _keyStatus |= MovementKeyStatus.RIGHT;
                _nextDir = Directions.RIGHT;
                _parentTransform.eulerAngles = Vector3.zero;
            }
            else
            {
                _keyStatus &= ~MovementKeyStatus.RIGHT;
                RestoreMovementState();
            }
        }

        private void RestoreMovementState()
        {
            _nextDir = Directions.OFF;
            if ((_keyStatus & MovementKeyStatus.LEFT) == MovementKeyStatus.LEFT)
            {
                _nextDir = Directions.LEFT;
                _parentTransform.eulerAngles = _leftRotation;
            }
            else if((_keyStatus & MovementKeyStatus.RIGHT) == MovementKeyStatus.RIGHT)
            {
                _nextDir = Directions.RIGHT;
                _parentTransform.eulerAngles = Vector3.zero;
            }
            else if(_keyStatus == MovementKeyStatus.OFF)
            {
                _movState = MovementState.STOP_REQ;
            }
        }

        public bool IsGrounded()
        {
            if(_groundedCheckBox == null)
            {
                return false;
            }
            return (Physics2D.OverlapBox(_groundedCheckBox.position, _groundedCheckBox.localScale, 0, _groundLayer) != null);
        }
    }
}
