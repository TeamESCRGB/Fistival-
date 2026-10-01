using ComponentModule;
using Coordinator;
using Coordinator.Objects;
using Coordinator.Objects.Weapons;
//using Coordinator.Rhythm;
using Defines;
using Manager;
using Objects.Weapons;
using System.Collections;
using UnityEngine;

public class TestMusic : MonoBehaviour//, IExactRhythmReceiver
{
    public ModeTypes active;
    public ModeManageCoordinator mod;
    public ObjectCoordinator[] obc;
    public HeavyMachineGun[] machinegun;
    public HolySword[] sw;
    public Bicycle[] bccccc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Managers.Instance.ResourceManager.LoadAsyncAllIn("TestPreload", (_, a, b) =>
        {
            if(a==b)
            {
                Managers.Instance.DataManager.Init();
                Managers.Instance.GlobalSoundManager.Init();
                Managers.Instance.StageManager.Init();
                Managers.Instance.StageManager.SetStageIDX(0);

                FindAnyObjectByType<PlayerCoordinator>().Init();

                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.FISTIVAL);
                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.METROIDVANIA);
                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.PLATFORMER);
                //FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.RHYTHM);
                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.ROOT_SHOOTER);
                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.SHOOT_2D);
                FindAnyObjectByType<ModeManageCoordinator>().UnlockMode(ModeTypes.WWE);

                foreach(var o in obc)
                {

                    o.Init(Managers.Instance.DataManager.ObjectDataDict[0]);
                }

                foreach (var o in machinegun)
                {

                    o.Init(Managers.Instance.DataManager.ObjectDataDict[1]);
                }
                foreach (var o in sw)
                {

                    o.Init(Managers.Instance.DataManager.ObjectDataDict[2]);
                }
                foreach (var o in bccccc)
                {

                    o.Init(Managers.Instance.DataManager.ObjectDataDict[3]);
                }
                StartCoroutine(fstart());
            }
        });
        Managers.Instance.ResourceManager.LoadAsyncAllIn("StaticLoaded", null);
        Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", null);

    }



    public Renderer ren;

    IEnumerator fstart()
    {
        yield return new WaitForSeconds(1);
      

        //yield return new WaitForSeconds(2);

        //Managers.Instance.RhythmModeManager.PausePattern();
        //Debug.Log("p");
        //yield return new WaitForSeconds(2);
        //Debug.Log("s");
        //Managers.Instance.RhythmModeManager.UnPausePattern();
    }

    int cnt = 0;

    [ContextMenu("asdf")]
    void F()
    {
        mod.ChangeMode(active);
    }

    //public NoteTypes ty;

    
}
