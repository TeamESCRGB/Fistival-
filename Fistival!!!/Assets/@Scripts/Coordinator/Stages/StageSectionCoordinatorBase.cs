using UnityEngine;
using System.Collections.Generic;
using System;
using Manager;

namespace Coordinator.Stages
{
    public class StageSectionCoordinatorBase : MonoBehaviour
    {
        [SerializeField]
        private Transform _removeField;
        [SerializeField]
        private LayerMask _removeTargetLayerMask;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Assert(_removeField != null, $"{name}에 @RemoveField가 없습니다.");
#endif
        }

        public virtual void InitChunk()
        {
            Debug.Log($"InitChunk of {gameObject.name}");
        }

        public virtual void DeInitChunk()
        {
            Debug.Log($"DeinitChunk of {gameObject.name}");
        }

        public virtual void ClearAllObjects()
        {
            var result = Physics2D.OverlapBoxAll(_removeField.position, _removeField.lossyScale, 0, _removeTargetLayerMask);

            foreach(var obj in result)
            {
                Managers.Instance.ResourceManager.Destroy(obj.gameObject, true);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // _removeField가 할당되어 있지 않으면 실행하지 않음
            if (_removeField == null) return;

            // 원래 기즈모 매트릭스 저장
            Matrix4x4 originalMatrix = Gizmos.matrix;

            // _removeField의 위치, 회전, 월드 크기(lossyScale)를 기즈mo에 적용
            Gizmos.matrix = Matrix4x4.TRS(_removeField.position, _removeField.rotation, _removeField.lossyScale);

            // 기즈모 색상 지정
            Gizmos.color = Color.green;

            // 중심점 Vector3.zero(0,0,0)에서 1x1x1 크기의 와이어프레임 박스를 그림
            // (앞서 지정한 매트릭스 덕분에 lossyScale 크기로 자동 변환됩니다)
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            // 사용이 끝난 후 원래 기즈모 매트릭스로 복구
            Gizmos.matrix = originalMatrix;
        }
#endif

    }
}