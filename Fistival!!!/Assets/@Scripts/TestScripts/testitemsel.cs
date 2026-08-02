using Coordinator;
using Manager;
using UnityEngine;

public class testitemsel : MonoBehaviour
{
    public PlayerCoordinator pc;
    public int a;
    [ContextMenu("ch")]
    void f()
    {
        pc.EquipItem(0, a);
        Debug.Log(Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.EquippedItems[0]);
    }
}
