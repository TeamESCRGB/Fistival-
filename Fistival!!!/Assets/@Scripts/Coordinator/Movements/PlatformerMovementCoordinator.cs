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
    public class PlatformerMovementCoordinator : MonoBehaviour ,IJumpsMovementInputHandler, IHorizontalMovementInputHandler, IPushable, IMovementLockable
    {
        [SerializeField]
        private float _platformIgnoreTime = 0.5f;
        
        [SerializeField]
        protected LayerMask _platformMask;
        [SerializeField]
        protected LayerMask _groundLayer;

        private Transform _groundedCheckBox;
        

        private Collider2D _parentCol;
        protected Rigidbody2D _parentRb2d;
        private Transform _parentTransform;
        private float _speed;
        private float _jumpPow;
        private float _slowness;

        private float _slownessSensitivity=1;
        private float _maxSlowness=0;

        private float _rightRot = 0;
        private float _leftRot = 180;

        private WaitForSeconds _platformEnableDelay;

        [SerializeField]private MovementKeyStatus _keyStatus = MovementKeyStatus.OFF;
        private Directions _nextDir;
        [SerializeField]private MovementState _movState;
        protected bool _isInputLocked;
        private int _lockCnt = 0;
        
        [SerializeField]
        private float _coyoteTime = 0.1f;
        protected float _coyoteTimeCounter = -1;

        [SerializeField]
        protected float _jumpBufferTime = 0.12f;
        protected float _jumpBufferCounter = -1;

        private bool _isGrounded = false;

        private bool _isGravityFlipped = false;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _platformEnableDelay = new WaitForSeconds(_platformIgnoreTime);
            _groundedCheckBox = transform.Find("@GroundedCheckBox");
        }

        public virtual void Init(float speed,float jumpPow ,float slownessSensitivity,float maxSlowness,Rigidbody2D parentRb2d)
        {
            _isGravityFlipped = parentRb2d.gravityScale < 0;

            if(_isGravityFlipped)
            {
                float tmp = _rightRot;
                _rightRot = _leftRot;
                _leftRot = tmp;
            }

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
            _isInputLocked = false;
            _jumpBufferCounter = -1;
            _coyoteTimeCounter = -1;
            _lockCnt = 0;
        }

        private void FixedUpdate()
        {
            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }

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

            if(_isGrounded && _jumpBufferCounter>=0 && (_isInputLocked == false))
            {
                Jump();
            }

            if(_parentRb2d == null)
            {
                return;
            }


            //이거는 어쨌든 이동 입력이 있을때만 작동하는데, 그러면 락이 걸리지 않고, 입력이 있을때만 작동하게 하면 안되나?
            if(_isInputLocked == false)
            {
                _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed * _slowness, (float)_nextDir, ref _movState);
            }
        }

        //락 해제
        //먼저, 속도부터 복구
        //복구되면 안되는 상황:
        //입력된 키가 없음, STOP_REQ인 상황
        public void LockMovement()
        {
            _lockCnt++;
            if(_isInputLocked)
            {
                return;
            }
            if(_movState != MovementState.START_REQ && _movState != MovementState.STOP_ACCEPT)
            {
                MovementState stopReq = MovementState.STOP_REQ;
                _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed * _slowness, 0, ref stopReq);
            }
            _isInputLocked = true;
            //inputlock플레그 올리기
            //속도를 빼면 안되는 상황:
            //쿨다운이 안끝났는데, 다시 요청 들어옴, START_REQ인 상황, STOP_REQ_ACCEPT인 상황

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

            if (_movState == MovementState.STOP_REQ)
            {
                _movState = MovementState.STOP_ACCEPT;
            }

            _parentRb2d.linearVelocityX = MovementUtils.CalculateNewSpeed(_parentRb2d.linearVelocityX, _speed * _slowness, (float)_nextDir, ref _movState);
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
            if(pressed == false || _parentRb2d == null || _parentCol == null || _isInputLocked)
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
            Vector2 force;

            if(_isGravityFlipped)
            {
                force = Vector2.down* _jumpPow;
            }
            else
            {
                force = Vector2.up* _jumpPow;
            }

            _parentRb2d.AddForce(force, ForceMode2D.Impulse);
        }

        public virtual void OnJumpMovementInputEvent(bool pressed)
        {
            if(pressed == false)
            {
                return;
            }
            if ((IsGrounded() || _coyoteTimeCounter >= 0) && (_isInputLocked == false))
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
                Vector3 eularAngle = _parentTransform.eulerAngles;
                eularAngle.y = _leftRot;
                _parentTransform.eulerAngles = eularAngle;
            }
            else if((_keyStatus & MovementKeyStatus.LEFT) == MovementKeyStatus.LEFT)
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
                Vector3 eularAngle = _parentTransform.eulerAngles;
                eularAngle.y = _rightRot;
                _parentTransform.eulerAngles = eularAngle;
            }
            else if((_keyStatus & MovementKeyStatus.RIGHT) == MovementKeyStatus.RIGHT)
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
                Vector3 eularAngle = _parentTransform.eulerAngles;
                eularAngle.y = _leftRot;
                _parentTransform.eulerAngles = eularAngle;
            }
            else if((_keyStatus & MovementKeyStatus.RIGHT) == MovementKeyStatus.RIGHT)
            {
                _nextDir = Directions.RIGHT;
                Vector3 eularAngle = _parentTransform.eulerAngles;
                eularAngle.y = _rightRot;
                _parentTransform.eulerAngles = eularAngle;
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
