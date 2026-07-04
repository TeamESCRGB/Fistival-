using Coordinator.Movements;
using Defines;
using UnityEngine;

namespace Coordinator.TriggerMovement
{
    public class PlatfoermerTriggerMovementCoordinator : MonoBehaviour
    {
        private float _speed;
        private Transform _target;
        private bool _isFollowOn = false;
        private PlatformerMovementCoordinator _platformerMov;
        private void Awake()
        {
            _platformerMov = GetComponent<PlatformerMovementCoordinator>();
        }

        public void Init(float speed,  float jumpPower, Rigidbody2D rb2d, MovementKeyStatus initialDir)
        {
            _platformerMov.Init(speed, jumpPower, 0, 1, rb2d);
            _target = null;
            _isFollowOn = false;
            _speed = speed;

            switch(initialDir)
            {
                case MovementKeyStatus.RIGHT:
                    _platformerMov.OnLeftMovementInputEvent(false);
                    _platformerMov.OnRightMovementInputEvent(true);
                    break;
                case MovementKeyStatus.LEFT:
                    _platformerMov.OnRightMovementInputEvent(false);
                    _platformerMov.OnLeftMovementInputEvent(true);
                    break;
            }
        }
        public void SetNowSpeed(Vector2 linearSpeed)
        {
            _platformerMov.SetNowSpeed(linearSpeed);
        }
        public void FollowTarget(float speed, Transform target)
        {
            _isFollowOn=true;
            _target = target;
            _platformerMov.SetMaxSpeed(speed);
        }

        public void StopFollow()
        {
            _isFollowOn=false;
            _platformerMov.SetMaxSpeed(_speed);
            _target = null;
        }

        private void Update()
        {
            if(_isFollowOn)
            {
                if(_target == null)
                {
                    return;
                }
                if(_target.position.x < transform.position.x)
                {
                    _platformerMov.OnRightMovementInputEvent(false);
                    _platformerMov.OnLeftMovementInputEvent(true);
                }
                else
                {
                    _platformerMov.OnLeftMovementInputEvent(false);
                    _platformerMov.OnRightMovementInputEvent(true);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == null)
            {
                return;
            }

            if (_isFollowOn)
            {
                return;
            }

            if(collision.CompareTag("RightMovementTag"))
            {
                _platformerMov.OnLeftMovementInputEvent(false);
                _platformerMov.OnRightMovementInputEvent(true);
            }
            else if(collision.CompareTag("LeftMovementTag"))
            {
                _platformerMov.OnRightMovementInputEvent(false);
                _platformerMov.OnLeftMovementInputEvent(true);
            }
            else if (collision.CompareTag("JumpTag"))
            {
                _platformerMov.OnJumpMovementInputEvent(true);
            }
            else if (collision.CompareTag("DownJumpTag"))
            {
                _platformerMov.OnDownJumpMovementInputEvent(true);
            }
            else if(collision.CompareTag("StopTag"))
            {
                _platformerMov.OnLeftMovementInputEvent(false);
                _platformerMov.OnRightMovementInputEvent(false);
            }
        }
    }
}