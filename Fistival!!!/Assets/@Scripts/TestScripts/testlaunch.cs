using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class testlaunch : MonoBehaviour
{
    public Rigidbody2D _rb2d;
    public float totalMoveTime;

    public Transform target;
    public static Vector2 CalculateThrowPower(Vector2 distance, float totalMoveTime, float linearDamping, float dampingThreshold, float gravityAbsolute, in Vector2 currentVelocity)
    {
        Vector2 dis = distance;
        float t = totalMoveTime;

        if (t <= 0.001f) return Vector2.zero;

        float d = linearDamping;
        Vector2 targetSpd = Vector2.zero;

        // 공기 저항(Damping)이 거의 없을 때 (기본 등가속도 공식)
        if (d <= dampingThreshold)
        {
            targetSpd.x = dis.x / t;
            targetSpd.y = (dis.y / t) + (0.5f * gravityAbsolute * t);
        }
        // 공기 저항이 존재할 때 (정확한 미분방정식 역산)
        else
        {
            // 공기 저항에 의해 감쇠되는 핵심 배수 (e^-dt)
            float expFactor = Mathf.Exp(-d * t);

            // 1. 가로(X) 축 초기 속도 역산
            targetSpd.x = (dis.x * d) / (1.0f - expFactor);

            // 2. 세로(Y) 축 초기 속도 역산 (중력은 저항과 분리하여 수학적으로 정확히 매칭)
            // 유니티 2D/3D 기본 중력은 음수(-) 방향이므로 아래 식 구조가 완벽히 대응됩니다.
            float numeratorY = dis.y * d + gravityAbsolute * t - (gravityAbsolute / d) * (1.0f - expFactor);
            targetSpd.y = numeratorY / (1.0f - expFactor);
        }

        // 3. 현재 속도를 빼주어 최종 충격량(Impulse) 산출
        Vector2 impulseForce = targetSpd - currentVelocity;
        return impulseForce;
    }

    [ContextMenu("f")]
    void f()
    {
        StartCoroutine(sdaf());
    }

    IEnumerator sdaf()
    {
        yield return new WaitForSeconds(0.5f);
        Vector2 impulseForce = CalculateThrowPower(target.position - transform.position, totalMoveTime, _rb2d.linearDamping, 0.001f, Mathf.Abs(Physics2D.gravity.y), _rb2d.linearVelocity);

        _rb2d.AddForce(impulseForce, ForceMode2D.Impulse);
    }
}
