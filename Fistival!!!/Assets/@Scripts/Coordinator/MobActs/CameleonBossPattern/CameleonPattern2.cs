using Coordinator;
using Coordinator.Movements;
using Defines;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;


namespace MobActs.CameleonBossPattern
{
    public class CameleonPattern2 : MobActBase
    {
        private IReadOnlyList<Transform> _points;
        private Vector2 _fieldCenterPos;
        private bool _isActing;
        private bool _isMoving;
        private MovementKeyStatus _dir;
        private float _interval;
        private float _intervalCounter;
        private float _moveTime;
        private int _movIdx;
        private Rigidbody2D _rb2d;
        private PointMovement _mov;

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d, PointMovement mov, float movInterval, float moveTime, IReadOnlyList<Transform> points, Vector2 fieldCenterPos)
        {
            Init(onActionEnd, animator);

            _points=points;

            _fieldCenterPos = fieldCenterPos;
            _dir = MovementKeyStatus.LEFT;
            _isActing = false;
            _isMoving = false;
            _interval = movInterval;
            _moveTime = moveTime;
            _rb2d = rb2d;
            _mov = mov;
        }

        private void FixedUpdate()
        {
            if (_isActing == false || _isMoving)
            {
                return;
            }

            if (_intervalCounter < _interval)
            {
                _intervalCounter += Time.fixedDeltaTime;
                return;
            }
            _intervalCounter = 0;
            _isMoving = true;
            _mov.ReqStartMove(_points[_movIdx], _moveTime, _rb2d, OnMovEndCb);

        }

        public void EndCameleonPattern2()
        {
            _isActing = false;
            _isMoving = false;
            _onActEnd?.Invoke();
        }

        private void OnMovEndCb(bool result)
        {
            if (result == false)
            {
                return;
            }

            if(_dir == MovementKeyStatus.RIGHT)
            {
                _movIdx++;
            }
            else
            {
                _movIdx--;
            }

            if (_movIdx >= _points.Count || _movIdx < 0)
            {
                _animator.SetTrigger("CameleonPattern2End");
                return;
            }
            _isMoving = false;
        }

        public void StartCameleonPattern1()
        {
            _isMoving = false;
            var pos = _rb2d.position;
            if (pos.x < _fieldCenterPos.x)
            {
                _dir = MovementKeyStatus.RIGHT;
                var rot = _rb2d.transform.rotation;
                rot.y = 0;
                _rb2d.transform.rotation = rot;
                _movIdx = 1;
            }
            else
            {
                _dir = MovementKeyStatus.LEFT;
                var rot = _rb2d.transform.rotation;
                rot.y = 180;
                _rb2d.transform.rotation = rot;
                _movIdx = _points.Count - 2;
            }
        }

        public override void Act()
        {
            _intervalCounter = 0;
            _movIdx = 0;
            _isActing = true;
            _isMoving = true;
            _animator.SetTrigger("CameleonPattern2");
        }

        public override void StopAct()
        {
            if (_isActing == false)
            {
                return;
            }
            _isActing = false;
            _isMoving = false;
            _mov.StopMove();
        }
    }
}