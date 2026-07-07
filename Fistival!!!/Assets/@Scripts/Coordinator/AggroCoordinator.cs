using System;
using UnityEngine;

namespace Coordinator
{
    public class AggroCoordinator : MonoBehaviour
    {
        private float _checkThreshold=0.1f;
        private float _checkTime = 0;
        private Transform _checkRange;
        private Action<bool, Collider2D> _onStateChanged;
        private LayerMask _playerLayer;
        private LayerMask _blockLayer;
        private bool _isAggroOn = false;

        private void Awake()
        {
            _checkRange = transform;
        }

        public void Init(float checkRateSec,Action<bool, Collider2D> OnStateChanged, LayerMask playerLayer, LayerMask groundLayer)
        {
            _checkThreshold = checkRateSec;
            _isAggroOn = false;
            _checkTime = _checkThreshold;
            _onStateChanged = OnStateChanged;
            _playerLayer = playerLayer;
            _blockLayer = groundLayer;
        }

        private void LateUpdate()
        {
            _checkTime += Time.deltaTime;
            if(_checkTime < _checkThreshold)
            {
                return;
            }

            _checkTime = 0;
            var pos = _checkRange.position;
            var result = Physics2D.OverlapBox(pos, _checkRange.lossyScale,_checkRange.rotation.z,_playerLayer);
            bool aggro = false;

#if UNITY_EDITOR
            _latestPlayerCollider = result;
            _latestCastPos = pos;
            _latestHit = default; // 초기화
#endif

            if (result != null)
            {
                var dir = result.transform.position - pos;

                var hit = Physics2D.Raycast(pos, dir, dir.magnitude, _blockLayer);

                Collider2D collider = hit.collider;

                aggro = collider == null;

#if UNITY_EDITOR
                _latestHit = hit;
#endif
            }

            if(aggro != _isAggroOn)
            {
                _onStateChanged?.Invoke(aggro, result);
                _isAggroOn = aggro;
            }
        }

#if UNITY_EDITOR
        private Collider2D _latestPlayerCollider = null;
        private RaycastHit2D _latestHit;
        private Vector3 _latestCastPos;

        private void OnDrawGizmos()
        {
            // 게임이 실행 중일 때만 시각화
            if (!Application.isPlaying) return;

            // 1. OverlapBox 감지 영역 그리기 (기존 코드)
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity; // 행렬 원복

            // 2. LateUpdate에서 저장한 멤버 변수를 기반으로 Ray 그리기
            if (_latestPlayerCollider != null)
            {
                if (_latestHit.collider != null)
                {
                    // 장애물에 막힌 경우: 빨간색 실선 + 부딪힌 지점에 구체 표시
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(_latestCastPos, _latestHit.point);
                    Gizmos.DrawWireSphere(_latestHit.point, 0.1f);
                }
                else
                {
                    // 시야가 완전히 확보된 경우: 청록색 실선으로 플레이어까지 연결
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(_latestCastPos, _latestPlayerCollider.transform.position);
                }
            }
        }
#endif

    }
}