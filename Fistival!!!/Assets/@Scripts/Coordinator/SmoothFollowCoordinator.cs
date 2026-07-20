using UnityEngine;

namespace Coordinator
{
    public class SmoothFollowCoordinator : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _followRate;
        [SerializeField]
        private Vector2 _deadzoneWidth;
        [SerializeField]
        private Vector2 _deadzoneHeight;
        [SerializeField]
        private float _threshold = 0.001f;
        private bool _followState = true;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetLeftBorder(float left)
        {
            _deadzoneWidth.x = left < 0 ? 0 : left;
        }

        public void SetRightBorder(float right)
        {
            _deadzoneWidth.y = right < 0 ? 0 : right;
        }

        public void SetUpBorder(float up)
        {
            _deadzoneHeight.y = up < 0 ? 0 : up;
        }

        public void SetDownBorder(float down)
        {
            _deadzoneHeight.x = down < 0 ? 0 : down;
        }

        public void SetFollowState(bool state)
        {
            _followState = state;
        }

        private void LateUpdate()
        {
            if (_target == null || _followState == false)
            {
                return;
            }

            Vector2 distance = Vector2.zero;
            Vector2 tarPos = _target.position;
            Vector2 myPos = transform.position;

            Vector2 widthBound = new Vector2(myPos.x - _deadzoneWidth.x, myPos.x + _deadzoneWidth.y);
            Vector2 heightBound = new Vector2(myPos.y - _deadzoneHeight.x, myPos.y + _deadzoneHeight.y);

            if (tarPos.x < widthBound.x)
            {
                distance.x = tarPos.x - widthBound.x;
            }
            else if (tarPos.x > widthBound.y)
            {
                distance.x = tarPos.x - widthBound.y;
            }

            if (tarPos.y < heightBound.x)
            {
                distance.y = tarPos.y - heightBound.x;
            }
            else if (tarPos.y > heightBound.y)
            {
                distance.y = tarPos.y - heightBound.y;
            }

            if (Mathf.Abs(distance.x) < _threshold)
            {
                distance.x = 0;
            }

            if (Mathf.Abs(distance.y) < _threshold)
            {
                distance.y = 0;
            }

            if (distance == Vector2.zero)
            {
                return;
            }
            transform.Translate(distance * _followRate * Time.deltaTime, Space.World);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 게임이 실행 중일 때만 그리도록 설정 (위치 값을 정확히 잡기 위함)
            if (!Application.isPlaying) return;

            // Gizmos 색상을 초록색으로 설정 (원하는 색상으로 변경 가능)
            Gizmos.color = Color.green;

            // 현재 카메라(오브젝트)의 위치
            Vector3 myPos = transform.position;

            // 좌, 우, 상, 하 경계선 좌표 계산
            float left = myPos.x - _deadzoneWidth.x;
            float right = myPos.x + _deadzoneWidth.y;
            float bottom = myPos.y - _deadzoneHeight.x;
            float top = myPos.y + _deadzoneHeight.y;

            // 사각형의 네 모서리 점 지정
            Vector3 topLeft = new Vector3(left, top, myPos.z);
            Vector3 topRight = new Vector3(right, top, myPos.z);
            Vector3 bottomLeft = new Vector3(left, bottom, myPos.z);
            Vector3 bottomRight = new Vector3(right, bottom, myPos.z);

            // 네 모서리를 선으로 연결하여 사각형 그리기
            Gizmos.DrawLine(topLeft, topRight);     // 윗변
            Gizmos.DrawLine(topRight, bottomRight); // 우변
            Gizmos.DrawLine(bottomRight, bottomLeft); // 밑변
            Gizmos.DrawLine(bottomLeft, topLeft);   // 좌변
        }
#endif
    }
}