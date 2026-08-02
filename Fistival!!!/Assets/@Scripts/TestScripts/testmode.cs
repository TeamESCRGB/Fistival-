using Coordinator;
using Defines;
using Manager.Contents;
using UnityEngine;

public class testmode : MonoBehaviour
{
    public ModeManageCoordinator mod;
    public ModeTypes mode;
    [ContextMenu("change")]
    void f()
    {
        mod.UnlockMode(mode);
        mod.ChangeMode(mode);
    }
}
