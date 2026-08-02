using System.Collections.Generic;
using UnityEngine;

public class testwave : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;       // 파동이 오른쪽으로 이동하는 속도
    public float amplitude = 2f;    // 파동의 높이
    public float width = 5f;        // 파동의 영향 범위 (폭)

    private List<Transform> boxTransforms = new List<Transform>();
    private List<float> originalYs = new List<float>();
    private float currentCenter = -10f;
    private bool isRunning = false;

    void Start()
    {
        // 자식 오브젝트들을 리스트에 담고 초기 Y 위치 저장
        foreach (Transform child in transform)
        {
            boxTransforms.Add(child);
            originalYs.Add(child.position.y);
        }
    }

    void Update()
    {
        if (!isRunning) return;

        // 1. 파동 중심 이동
        currentCenter += speed * Time.deltaTime;

        // 2. 종료 조건 체크: 파동이 마지막 박스를 지나갔는지 확인
        if (boxTransforms.Count > 0)
        {
            float lastBoxX = boxTransforms[boxTransforms.Count - 1].position.x;
            if (currentCenter > lastBoxX + width)
            {
                ResetAllBoxes();
                isRunning = false;
                return;
            }
        }

        // 3. 모든 박스 순회하며 높이 계산
        for (int i = 0; i < boxTransforms.Count; i++)
        {
            float dist = boxTransforms[i].position.x - currentCenter;

            // 파동 범위 안에 있을 때만 연산
            if (Mathf.Abs(dist) < width)
            {
                float t = dist / width;
                // 이차함수 공식: 1 - t^2
                float height = amplitude * (1 - (t * t));

                Vector3 pos = boxTransforms[i].position;
                boxTransforms[i].position = new Vector3(pos.x, originalYs[i] + height, pos.z);
            }
            else
            {
                // 범위 밖은 원래 위치로 복귀
                Vector3 pos = boxTransforms[i].position;
                if (Mathf.Abs(pos.y - originalYs[i]) > 0.001f)
                {
                    boxTransforms[i].position = new Vector3(pos.x, originalYs[i], pos.z);
                }
            }
        }
    }

    private void ResetAllBoxes()
    {
        for (int i = 0; i < boxTransforms.Count; i++)
        {
            Vector3 pos = boxTransforms[i].position;
            boxTransforms[i].position = new Vector3(pos.x, originalYs[i], pos.z);
        }
    }

    [ContextMenu("Trigger Wave")]
    public void TriggerWave()
    {
        // 시작 위치를 첫 번째 박스보다 앞쪽(왼쪽)으로 설정
        currentCenter = boxTransforms[0].position.x - width;
        isRunning = true;
    }
}
