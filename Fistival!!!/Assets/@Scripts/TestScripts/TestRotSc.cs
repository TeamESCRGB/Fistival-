

using Coordinator.Chain;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TestRotSc : MonoBehaviour
{
    public Transform tar;
    public Vector2 dir;
    public Rigidbody2D rb;
    public float t;

    IEnumerator st()
    {
        yield return new WaitForSeconds(1);

        Vector2 targetSpd = dir.normalized;
        
        targetSpd.x = targetSpd.x/ t;

        targetSpd.y = targetSpd.y/ t;

        rb.linearVelocity = targetSpd;
    }

    [ContextMenu("start")]
    void fun()
    {
        StartCoroutine(st());
    }

    private void FixedUpdate()
    {
        Vector2 my = transform.position;
        Vector2 tart = tar.position;
        float len = (my - tart).magnitude;

        tar.localScale = new Vector2(len,1);
        
    }
    public float len;


    public ChainMorningStar ca;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("a");
    }
}