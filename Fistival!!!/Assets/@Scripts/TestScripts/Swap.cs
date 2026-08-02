using Manager;
using UnityEngine;

public class Swap : MonoBehaviour
{
    public Transform a, b;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Managers.Instance.ResourceManager.LoadAsyncAllIn("StaticLoaded", (_, a, b) =>
        {
            if (a == b)
            {
                Managers.Instance.DataManager.Init();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
