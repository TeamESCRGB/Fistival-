using Coordinator.Movements;
using System;
using System.Collections;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class JumpMovementAct : MobActBase
    {
        private Rigidbody2D _rb2d;
        private float _maxSpeed=0;
        private PlatformerMovementCoordinator _mov;
        private Transform _player;
        private WaitForSeconds _jumpDelay;

        private bool _isMoving;

        private bool _isActing;

        public void Init(Action onEnd, Animator animator, PlatformerMovementCoordinator movCoord, Rigidbody2D rb2d, float maxSpeed,float jumpDelay)
        {
            Init(onEnd,animator);
            _isActing = false;
            _maxSpeed = maxSpeed;
            _rb2d = rb2d;
            _mov = movCoord;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            _jumpDelay = new WaitForSeconds(jumpDelay);
            _isMoving = false;
        }

        public void Move()
        {
            _mov.SetMaxSpeed(_maxSpeed);
            if (_player.position.x < transform.position.x)
            {
                _mov.OnLeftMovementInputEvent(true);
            }
            else
            {
                _mov.OnRightMovementInputEvent(true);
            }
            _animator.SetTrigger("Move");
        }

        public void Jump()
        {
            _mov.SetMaxSpeed(0);
            _mov.OnLeftMovementInputEvent(false);
            _mov.OnRightMovementInputEvent(false);
            _mov.OnJumpMovementInputEvent(true);
            _isMoving = true;
            _animator.SetTrigger("Jump");
        }
        private IEnumerator ActRoutine()
        {
            Move();
            yield return _jumpDelay;
            Jump();
        }

        private void FixedUpdate()
        {
            if(_isMoving == false)
            {
                return;
            }

            if(_rb2d.linearVelocity.magnitude < 0.0001f)
            {
                End();
            }
        }

        private void End()
        {
            _rb2d.linearVelocity = Vector2.zero;
            _isMoving = false;
            _onActEnd?.Invoke();
        }

        public override void Act()
        {
            _isActing = true;
            _routine = StartCoroutine(ActRoutine());
        }

        public override void StopAct()
        {
            if(_routine != null && _isActing)
            {
                StopCoroutine(_routine);
            }
            End();
        }
    }
}