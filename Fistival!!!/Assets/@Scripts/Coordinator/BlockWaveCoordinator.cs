using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator
{
    public class BlockWaveCoordinator : MonoBehaviour
    {
        private float _speed = 10f;       // 파동이 오른쪽으로 이동하는 속도
        private float _amplitude = 2f;    // 파동의 높이
        private float _width = 5f;        // 파동의 영향 범위 (폭)

        private List<Transform> _boxTransforms = new List<Transform>();
        private List<float> _originalYs = new List<float>();
        private float _currentCenter = -10f;
        private bool _isRunning = false;

        private event Action<bool> OnEnd;

        void Start()
        {
            // 자식 오브젝트들을 리스트에 담고 초기 Y 위치 저장
            foreach (Transform child in transform)
            {
                _boxTransforms.Add(child);
                _originalYs.Add(child.position.y);
            }
        }

        void Update()
        {
            if (!_isRunning) return;

            // 1. 파동 중심 이동
            _currentCenter += _speed * Time.deltaTime;

            // 2. 종료 조건 체크: 파동이 마지막 박스를 지나갔는지 확인
            if (_boxTransforms.Count > 0)
            {
                float lastBoxX = _boxTransforms[_boxTransforms.Count - 1].position.x;
                if (_currentCenter > lastBoxX + _width)
                {
                    ResetAllBoxes();
                    _isRunning = false;
                    OnEnd?.Invoke(true);
                    return;
                }
            }

            // 3. 모든 박스 순회하며 높이 계산
            for (int i = 0; i < _boxTransforms.Count; i++)
            {
                float dist = _boxTransforms[i].position.x - _currentCenter;

                // 파동 범위 안에 있을 때만 연산
                if (Mathf.Abs(dist) < _width)
                {
                    float t = dist / _width;
                    // 이차함수 공식: 1 - t^2
                    float height = _amplitude * (1 - (t * t));

                    Vector3 pos = _boxTransforms[i].position;
                    _boxTransforms[i].position = new Vector3(pos.x, _originalYs[i] + height, pos.z);
                }
                else
                {
                    // 범위 밖은 원래 위치로 복귀
                    Vector3 pos = _boxTransforms[i].position;
                    if (Mathf.Abs(pos.y - _originalYs[i]) > 0.001f)
                    {
                        _boxTransforms[i].position = new Vector3(pos.x, _originalYs[i], pos.z);
                    }
                }
            }
        }

        private void ResetAllBoxes()
        {
            for (int i = 0; i < _boxTransforms.Count; i++)
            {
                Vector3 pos = _boxTransforms[i].position;
                _boxTransforms[i].position = new Vector3(pos.x, _originalYs[i], pos.z);
            }
        }

        public void Stop()
        {
            if(_isRunning)
            {
                _isRunning = false;
                ResetAllBoxes();
                OnEnd?.Invoke(false);
            }
        }

        public void TriggerWave(float speed,float amplitude, float width)
        {
            _speed = speed;
            _amplitude = amplitude;
            _width = width;
            _currentCenter = _boxTransforms[0].position.x - width;
            _isRunning = true;
        }
    }
}
