using System.Collections;
using UnityEngine;

public class TestPlayerInit : MonoBehaviour
{

    IEnumerator func()
    {
        yield return new WaitForSeconds(3);
    }
}
