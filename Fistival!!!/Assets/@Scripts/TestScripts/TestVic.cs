using Coordinator;
using Coordinator.Victims;
using Manager;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestVic : MonoBehaviour
{
    [SerializeField]
    int hp = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<VictimCoordinator>().Init(hp, hp, 0,default);
        GetComponentInChildren<HPCoordinator>().SubscribeOnDead(() => { Managers.Instance.ResourceManager.Destroy(transform.parent.gameObject, true); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
