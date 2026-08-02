using UnityEngine;

public class FuckyouUnity : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log($"{gameObject.name} awake");
    }

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} enable");
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name} start");
    }


    private void OnApplicationQuit()
    {
        Debug.Log($"{gameObject.name} quit");
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} disable");
    }

    private void OnDestroy()
    {
        Debug.Log($"{gameObject.name} destroy");
    }
}
