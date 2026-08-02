using UnityEngine;

public class testanim : MonoBehaviour
{
    public Animator a;

    [ContextMenu("start")]
    void f()
    {
        a.Play("New State");
    }

    public void Fun()
    {
        Debug.Log("asdf");
    }
}
