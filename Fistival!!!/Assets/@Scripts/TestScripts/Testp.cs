using UnityEngine;

public class ReverseQuadraticVelocity2D : MonoBehaviour
{
    private Rigidbody2D rb2d; // 2D 리지드바디로 변경
    private float elapsedTime = 0f;

    [Header("이동 설정")]
    public float duration = 2.0f;       // 역주행 연출이 일어날 총 시간
    public float aValue = 3.0f;         // -a부터 a까지의 범위 결정

    public float progress;

    void Start()
    {
        // 2D 컴포넌트 가져오기
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (progress > 1.0f)
        {
            return;
        }

        elapsedTime += Time.fixedDeltaTime;
        progress = elapsedTime / duration; // 0 ~ 1 진행도

        float mappedX = Mathf.Lerp(-aValue, aValue, progress);
        float targetVelocityX = mappedX * mappedX;

        if(mappedX<0)
        {
            targetVelocityX = -targetVelocityX;
        }

        rb2d.linearVelocity = new Vector2(targetVelocityX, rb2d.linearVelocity.y);
    }
}
