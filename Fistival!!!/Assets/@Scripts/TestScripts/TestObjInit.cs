using Coordinator.Objects;
using Manager;
using UnityEngine;

public class TestObjInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int ObjIdx;
    void Start()
    {
        GetComponent<ObjectCoordinator>().Init(Managers.Instance.DataManager.ObjectDataDict[ObjIdx]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
