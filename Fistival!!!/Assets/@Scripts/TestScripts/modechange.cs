using Coordinator;
using Defines;
using UnityEngine;

public class modechange : MonoBehaviour
{
    ModeManageCoordinator mod;
    private void Awake()
    {
        mod = FindAnyObjectByType<ModeManageCoordinator>();
    }

    public ModeTypes t;
    [ContextMenu("a")]
    void f()
    {
        mod.UnlockMode(t);
        mod.ChangeMode(t);
    }
}
