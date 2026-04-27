using UnityEngine;

namespace Coordinator.Movements
{
    public class DoubleJumpMovementCoordinator : PlatformerMovementCoordinator
    {
        private bool _canDoubleJump = true;
        public override void Init(float speed, float jumpPow, float slownessSensitivity, float maxSlowness, Rigidbody2D parentRb2d)
        {
            base.Init(speed, jumpPow, slownessSensitivity, maxSlowness, parentRb2d);
            _canDoubleJump = true;
        }

        private void Start()
        {
            gameObject.GetComponentInChildren<PlatformerFootCoordinator>(true).OnStepKill -= Jump;
            gameObject.GetComponentInChildren<PlatformerFootCoordinator>(true).OnStepKill += Jump;
        }


        public override void OnJumpMovementInputEvent(bool pressed)
        {
            if (pressed == false)
            {
                return;
            }

            if (IsGrounded() || _coyoteTimeCounter >= 0)
            {
                Jump();
                _canDoubleJump = true;
            }
            else if(_canDoubleJump)
            {
                Jump();
                _canDoubleJump = false;
            }
            else
            {
                _jumpBufferCounter = _jumpBufferTime;
            }
        }
    }
}