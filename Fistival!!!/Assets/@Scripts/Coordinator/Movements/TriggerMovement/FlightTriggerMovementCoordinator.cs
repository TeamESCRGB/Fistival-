using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Coordinator.Movements.TriggerMovement
{
    public class FlightTriggerMovementCoordinator : MonoBehaviour
    {
        private float _speed;
        private Transform _target;
        private bool _isFollowOn = false;
        private FlightMovementCoordinator _flightMov;
        private void Awake()
        {
            _flightMov = GetComponent<FlightMovementCoordinator>();
        }

        public void Init(float speed, float jumpPower, Rigidbody2D rb2d)
        {
            _flightMov.Init(speed, rb2d);
            _target = null;
            _isFollowOn = false;
            _speed = speed;
        }

        public void FollowTarget(float speed, Transform target)
        {
            _isFollowOn = true;
            _target = target;
            _flightMov.SetMaxSpeed(speed);
        }

        public void StopFollow()
        {
            _isFollowOn = false;
            _flightMov.SetMaxSpeed(_speed);
            _target = null;
        }

        private void Update()
        {
            if (_isFollowOn)
            {
                if (_target == null)
                {
                    return;
                }

                Vector3 tarPos = _target.position;
                Vector3 pos = transform.position;

                if (tarPos.x < pos.x)
                {
                    _flightMov.OnRightMovementInputEvent(false);
                    _flightMov.OnLeftMovementInputEvent(true);
                }
                else
                {
                    _flightMov.OnLeftMovementInputEvent(false);
                    _flightMov.OnRightMovementInputEvent(true);
                }

                if (tarPos.y < pos.y)
                {
                    _flightMov.OnUpMovementInputEvent(false);
                    _flightMov.OnDownMovementInputEvent(true);
                }
                else
                {
                    _flightMov.OnDownMovementInputEvent(false);
                    _flightMov.OnUpMovementInputEvent(true);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == null)
            {
                return;
            }

            if(_isFollowOn)
            {
                return;
            }

            if (collision.CompareTag("RightMovementTag"))
            {
                _flightMov.OnLeftMovementInputEvent(false);
                _flightMov.OnRightMovementInputEvent(true);
            }
            else if (collision.CompareTag("LeftMovementTag"))
            {
                _flightMov.OnRightMovementInputEvent(false);
                _flightMov.OnLeftMovementInputEvent(true);
            }
            else if (collision.CompareTag("UpMovementTag"))
            {
                _flightMov.OnDownMovementInputEvent(false);
                _flightMov.OnUpMovementInputEvent(true);
            }
            else if (collision.CompareTag("DownMovementTag"))
            {
                _flightMov.OnUpMovementInputEvent(false);
                _flightMov.OnDownMovementInputEvent(true);
            }
            else if (collision.CompareTag("StopTag"))
            {
                _flightMov.OnLeftMovementInputEvent(false);
                _flightMov.OnRightMovementInputEvent(false);
                _flightMov.OnUpMovementInputEvent(false);
                _flightMov.OnDownMovementInputEvent(false);
            }
        }
    }
}
