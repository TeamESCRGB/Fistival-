using Coordinator;
using Coordinator.Movements;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;


namespace MobActs.CameleonBossPattern
{
    public class CameleonBossPattern2 : MobActBase
    {
        private IReadOnlyList<Transform> _points;
        private IReadOnlyList<Transform> _leftDownRightUp;
        private IReadOnlyList<Transform> _leftUpRightDown;
        private IReadOnlyList<Transform> _rightDownLeftUp;
        private IReadOnlyList<Transform> _rightUpLeftDown;
        private Vector2 _fieldCenterPos;
        private bool _isActing;
        private bool _isMoving;
        private float _interval;
        private float _intervalCounter;
        private float _moveTime;
        private int _movIdx;
        private Rigidbody2D _rb2d;
        private PointMovement _mov;

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d, PointMovement mov, float movInterval, float moveTime,
            IReadOnlyList<Transform> leftDownRightUp, IReadOnlyList<Transform> leftUpRightDown, IReadOnlyList<Transform> rightDownLeftUp, IReadOnlyList<Transform> rightUpLeftDown,
            Vector2 fieldCenterPos)
        {
            Init(onActionEnd, animator);

            _rightDownLeftUp = rightDownLeftUp;
            _rightUpLeftDown = rightUpLeftDown;
            _leftDownRightUp = leftDownRightUp;
            _leftUpRightDown = leftUpRightDown;

            _fieldCenterPos = fieldCenterPos;

            _isActing = false;
            _isMoving = false;
            _interval = movInterval;
            _moveTime = moveTime;
            _rb2d= rb2d;
            _mov = mov;
        }

        private void FixedUpdate()
        {
            if(_isActing==false || _isMoving)
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
            if(result == false)
            {
                return;
            }

            _movIdx++;

            if (_movIdx >= _points.Count)
            {
                _animator.SetTrigger("CameleonBossPattern2End");
                return;
            }
            _isMoving = false;
        }

        public void StartCameleonPattern1()
        {
            _isMoving = false;
            var pos = _rb2d.position;
            if(pos.x < _fieldCenterPos.x)
            {
                if(pos.y < _fieldCenterPos.y)
                {
                    _points = _leftDownRightUp;
                }
                else
                {
                    _points = _leftUpRightDown;
                }
            }
            else
            {
                if (pos.y < _fieldCenterPos.y)
                {
                    _points = _rightDownLeftUp;
                }
                else
                {
                    _points = _rightUpLeftDown;
                }
            }
        }

        public override void Act()
        {
            _intervalCounter = 0;
            _movIdx = 0;
            _isActing = true;
            _isMoving = true;
            _animator.SetTrigger("CameleonBossPattern2");
        }

        public override void StopAct()
        {
            if(_isActing == false)
            {
                return;
            }
            _isActing = false;
            _isMoving = false;
            _mov.StopMove();
        }
    }
}
