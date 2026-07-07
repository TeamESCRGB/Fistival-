using System;
using UnityEngine;

namespace Coordinator.Movements
{
    public class PointMovement : MonoBehaviour
    {
        private Action<bool> _onEnd;
        private bool _isMoving;
        private Rigidbody2D _rb2d;
        private Vector2 _movPow;
        private Vector2 _endPos;
        public void ReqStartMove(Transform point, float duration, Rigidbody2D rb2d, Action<bool> onEnd)
        {
            _onEnd = onEnd;
            _isMoving = true;
            _rb2d = rb2d;
            _endPos = new Vector2(point.position.x, point.position.y);

            // 전체 거리를 구함
            float totalDistance = Vector2.Distance(rb2d.position, _endPos);

            // duration이 0일 경우 예외 처리
            if (duration <= 0) duration = 0.001f;

            // 매 프레임 이동해야 할 속도 벡터 (거리 / 시간)
            Vector2 direction = (_endPos - rb2d.position).normalized;
            _movPow = direction * (totalDistance / duration);
        }

        public void StopMove()
        {
            _isMoving = false;
            _onEnd?.Invoke(false);
        }

        private void FixedUpdate()
        {
            if(_isMoving == false)
            {
                return;
            }

            var currentPos = _rb2d.position;

            Vector2 direction = (_endPos - currentPos).normalized;
            Vector2 remainingDistance = _endPos - currentPos;

            Vector2 moveStep = _movPow * Time.fixedDeltaTime;

            if (moveStep.sqrMagnitude >= remainingDistance.sqrMagnitude)
            {
                _isMoving = false;
                _rb2d.MovePosition(_endPos);
                _onEnd?.Invoke(true);
                return;
            }

            _rb2d.MovePosition(currentPos + moveStep);
        }
    }
}