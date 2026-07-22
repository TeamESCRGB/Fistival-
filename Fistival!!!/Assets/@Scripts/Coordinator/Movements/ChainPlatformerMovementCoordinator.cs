using ComponentModule;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Movements
{
    public class ChainPlatformerMovementCoordinator : PlatformerMovementCoordinator, IChainPullable
    {
        private float _gravityConstant;
        private LayerMask _groundLayermask;
        private FixedCooldownComponentModule _pullGroundDisableCounter;
        private Action _pullGroundDisableEndCallback;
        protected override void OnAwake()
        {
            base.OnAwake();
            _pullGroundDisableEndCallback = OnGroundDisableEnd;
        }

        private void OnDisable()
        {
            _parentRb2d.excludeLayers &= ~_groundLayermask;
            if (Managers.Instance != null && _pullGroundDisableCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnFixedModule(_pullGroundDisableCounter);
                _pullGroundDisableCounter = null;
            }
        }

        public override void Init(float speed, float jumpPow, float slownessSensitivity, float maxSlowness, Rigidbody2D parentRb2d)
        {
            base.Init(speed, jumpPow, slownessSensitivity, maxSlowness, parentRb2d);
            _gravityConstant = Mathf.Abs(Physics2D.gravity.y) * _parentRb2d.gravityScale;
            _groundLayermask = _platformMask | _groundLayer;
        }

        private void OnGroundDisableEnd()
        {
            _parentRb2d.excludeLayers &= ~_groundLayermask;
            if (Managers.Instance != null && _pullGroundDisableCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnFixedModule(_pullGroundDisableCounter);
                _pullGroundDisableCounter = null;
            }
        }

        public void Pull(Vector2 distance, float totalMoveTime, float dampingThreshold)
        {
            if (_pullGroundDisableCounter is not null)
            {
                _pullGroundDisableCounter.StopCooldown();
            }

            _parentRb2d.excludeLayers |= _groundLayermask;

            Vector2 impulseForce = MovementUtils.ApproximatedCalculateThrowPower(distance,totalMoveTime,_parentRb2d.linearDamping, dampingThreshold, _gravityConstant, _parentRb2d.linearVelocity);

            _parentRb2d.AddForce(impulseForce, ForceMode2D.Impulse);

            _pullGroundDisableCounter = Managers.Instance.CooldownManager.GetFixedCooldownModule(1 / _parentRb2d.linearVelocity.magnitude);

            _pullGroundDisableCounter.OnCooldownEnded += _pullGroundDisableEndCallback;
            _pullGroundDisableCounter.StartCooldown();
        }
    }
}