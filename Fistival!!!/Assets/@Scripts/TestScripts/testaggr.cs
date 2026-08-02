using Coordinator;
using UnityEngine;

public class testaggr : MonoBehaviour
{
    public LayerMask pl;
    public LayerMask ground;
    public bool aa;
    public float updateRate=0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("a")]
    void asdfStart()
    {
        GetComponent<AggroCoordinator>().Init(updateRate, (a, b) => { aa = a; }, pl,ground);   
    }
}
