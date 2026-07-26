using UnityEngine;
using Coordinator.Skills;
using Data.NonLodable;

namespace Coordinator.Movements
{
    public class DoubleJumpMovementCoordinator : PlatformerMovementCoordinator
    {
        private bool _canDoubleJump = true;
        public override void Init(float speed, float jumpPow, float slownessSensitivity, float maxSlowness, Rigidbody2D parentRb2d, MovementSoundKeys soundKeys)
        {
            base.Init(speed, jumpPow, slownessSensitivity, maxSlowness, parentRb2d, soundKeys);
            _canDoubleJump = true;
        }

        private void Start()
        {
            gameObject.GetComponentInChildren<PlatformerFootCoordinator>(true).OnStepAttackTriedBetweenFixedUpdate -= Jump;
            gameObject.GetComponentInChildren<PlatformerFootCoordinator>(true).OnStepAttackTriedBetweenFixedUpdate += Jump;
        }


        public override void OnJumpMovementInputEvent(bool pressed)
        {
            if (pressed == false)
            {
                return;
            }

            if ((IsGrounded() || _coyoteTimeCounter >= 0) && (_isInputLocked == false))
            {
                Jump();
                _canDoubleJump = true;
            }
            else if(_canDoubleJump && (_isInputLocked == false))
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