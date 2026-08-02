using Coordinator;
using Coordinator.Victims;
using Manager;
using UI;
using UnityEngine;

public class TestClear : MonoBehaviour
{
    [ContextMenu("cl")]
    void f()
    {
        Managers.Instance.StageManager.OnClear();
    }

    [ContextMenu("cl2")]
    void col()
    {
        Debug.Log(Managers.Instance.StageManager.CanCollect(6974));
        if(Managers.Instance.StageManager.CanCollect(6974)==false)
        {
            return;
        }
        Managers.Instance.StageManager.CollectCollection(6974);
    }

    [ContextMenu("dea")]
    void fai()
    {
        Managers.Instance.StageManager.OnDead();
    }

    [ContextMenu("checkhp")]
    void check()
    {
        Debug.Log(FindAnyObjectByType<PlayerVictimCoordinator>().GetComponent<HPCoordinator>().GetHP());
    }

    [ContextMenu("hp")]
    void hp()
    {
        FindAnyObjectByType<PlayerVictimCoordinator>().GetComponent<HPCoordinator>().SubtractHP(1);
    }

    public HPCoordinator boss;
    public VictimCoordinator b;
    [ContextMenu("boss")]
    void ffsadfg()
    {
        //FindAnyObjectByType<PlayerHUD>().SetBoss(boss,5);
    }

    [ContextMenu("bossini")]
    void inoi()
    {
        b.Init(5, 5, 5,default);
    }

    [ContextMenu("boss hit")]
    void bh()
    {
        //b.TakeDamage(1);
    }

}
