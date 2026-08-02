using UnityEngine;

public class testjumper : MonoBehaviour
{
    public bool a;

    [ContextMenu("jum")]
    void f()
    {
        Vector2 force;

        if(a)
        {
            force = Vector2.down * 5;
        }
        else
        {
            force = Vector2.up * 5;
        }

        GetComponent<Rigidbody2D>().AddForce(force,ForceMode2D.Impulse);
    }

    [ContextMenu("f")]
    void af()
    {
        transform.Rotate(180,0,0);
    }
}
