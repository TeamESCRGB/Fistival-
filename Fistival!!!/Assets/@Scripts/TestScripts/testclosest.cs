using UnityEngine;

public class testclosest : MonoBehaviour
{
    public Transform test;
    public Transform tar;
    // Update is called once per frame
    void Update()
    {
        test.position = GetComponent<Collider2D>().ClosestPoint(tar.transform.position);
    }
}
