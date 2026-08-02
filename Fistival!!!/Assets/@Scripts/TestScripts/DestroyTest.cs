using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [ContextMenu("asdf")]
    void call()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
