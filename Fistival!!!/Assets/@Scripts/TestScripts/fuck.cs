using ComponentModule;
using Coordinator.Movements;
using Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class fuck : MonoBehaviour, IChainPullable
{
    public Rigidbody2D rb;
    public Transform target;

    public float totMovTime;
    public LayerMask l;
    float g = 0;
    FixedCooldownComponentModule cbm;
    private void Start()
    {
        cb = () => { rb.excludeLayers &= ~l;

            Managers.Instance.CooldownManager.ReturnFixedModule(cbm);
            cbm = null;
        };
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    IEnumerator f2()
    {
        yield return new WaitForSeconds(1);
        rb.excludeLayers |= l;
        // 1. 위치 및 시간 변수 정의
        Vector2 startPos = transform.position;
        Vector2 endPos = target.position;
        Vector2 dis = endPos - startPos;
        float t = totMovTime;

        // 2. Linear Damping(항력) 보정 계수 계산
        float d = rb.linearDamping; // Unity 2023 이상은 linearDamping (구버전은 rb.drag)
        float dampingFactor = 1.0f;

        if (d > 0.001f)
        {
            // 항력으로 인해 줄어들 거리를 역산하여 속도를 뻥튀기해주는 물리 공식입니다.
            dampingFactor = (d * t) / (1.0f - Mathf.Exp(-d * t));
        }

        // 3. 항력이 없을 때의 기본 필요 속도 계산
        Vector2 targetSpd;
        targetSpd.x = dis.x / t;

        targetSpd.y = (dis.y / t) + (0.5f * g * t);

        // 4. 계산된 기본 속도에 항력 보정 계수(dampingFactor)를 곱해줍니다.
        targetSpd *= dampingFactor;

        // 5. 기존 관성(현재 속도) 반영 및 발사
        Vector2 currentSpd = rb.linearVelocity; // 구버전은 rb.velocity
        Vector2 impulseForce = targetSpd - currentSpd;

        rb.AddForce(impulseForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds((1+transform.localScale.x/2)/rb.linearVelocity.magnitude);
        rb.excludeLayers &= ~l;
        
        //일단 최종적인 코드
        //이게 hand에 붙어서, 타겟에 있는 rb를 가져와서 속도를 붙여준다
        //그리고 코루틴 하나 만들어서 다시 풀어주는것까지 만들고
        //아니면 타이머 하나 만들어서 하는것도 괜찮을듯. 이거는 즉시종료도 가능하니까
        //그러면 초기화에서 excludeLayers를 초기화하는 코드 작성 필요함
        //max = +-0.20.5
    }
    /*
     사슬 끝에 콜라이더를 놓고, 그거에 닿은것만 상호작용
    줄에는 닿아도 뭐가 없음

    끝 콜라이더를 이동시키고, 줄에 해당하는 오브젝트의 크기를 조정하는 방향으로 간다

     */

    IEnumerator asdfkljasfd()
    {
        yield return new WaitForSeconds(1);
        //Vector2 startPos = transform.position;
        //Vector2 endPos = target.position;
        //Vector2 dis = endPos - startPos;
        //Pull(dis, totMovTime, 0.001f);

        Vector2 startPos = pullT.position;
        Vector2 endPos = target.position;
        Vector2 dis = endPos - startPos;

        pull.Pull(dis,totMovTime,0.001f);
    }

    [ContextMenu("push")]
    void f1()
    {
        pull = pullT.GetComponent<IChainPullable>();
        StartCoroutine(f2());
    }

    Action cb;

    public Transform pullT;
    public IChainPullable pull;


    float time;

    private void FixedUpdate()
    {
        if(time <= 0)
        {
            return;
        }

        time -= Time.fixedDeltaTime;

        if(time <= 0)
        {
            cb?.Invoke();
        }
    }


    public void Pull(Vector2 distance, float totalMoveTime, float dampingThreshold)
    {
        if(cbm is not null)
        {
            cbm.StopCooldown();
            
        }
        rb.excludeLayers |= l;
        // 1. 위치 및 시간 변수 정의
        Vector2 dis = distance;
        float t = totalMoveTime;

        // 2. Linear Damping(항력) 보정 계수 계산
        float d = rb.linearDamping; // Unity 2023 이상은 linearDamping (구버전은 rb.drag)
        float dampingFactor = 1.0f;

        if (d > dampingThreshold)
        {
            // 항력으로 인해 줄어들 거리를 역산하여 속도를 뻥튀기해주는 물리 공식입니다.
            dampingFactor = (d * t) / (1.0f - Mathf.Exp(-d * t));
        }

        // 3. 항력이 없을 때의 기본 필요 속도 계산
        Vector2 targetSpd;
        targetSpd.x = dis.x / t;

        targetSpd.y = (dis.y / t) + (0.5f * g * t);

        // 4. 계산된 기본 속도에 항력 보정 계수(dampingFactor)를 곱해줍니다.
        targetSpd *= dampingFactor;

        // 5. 기존 관성(현재 속도) 반영 및 발사
        Vector2 currentSpd = rb.linearVelocity; // 구버전은 rb.velocity
        Vector2 impulseForce = targetSpd - currentSpd;

        rb.AddForce(impulseForce, ForceMode2D.Impulse);



        cbm = Managers.Instance.CooldownManager.GetFixedCooldownModule((1 + transform.localScale.x / 2) / rb.linearVelocity.magnitude);
        
        cbm.OnCooldownEnded += cb;
        cbm.StartCooldown();
    }
}