using ComponentModule;
using Manager;
using UI.Popup;
using UnityEngine;

public class TestCHeckbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Managers.Instance.ResourceManager.LoadAsyncAllIn("StaticLoaded", (_, a, b) =>
        //{
        //    if(a==b)
        //    {
        //        Managers.Instance.DataManager.Init();
        //    }
        //});
        //Managers.Instance.UIManager.Init();
    }

    [ContextMenu("b")]
    void f12()
    {
        Managers.Instance.UIManager.ShowPopupUI<SettingMenu>("SettingMenu");
    }

    [ContextMenu("a")]
    void func()
    {
        
        Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(a,b);
    }
    void a()
    {
        Managers.Instance.UIManager.ClosePopupUI();
        Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(a, b);
        Debug.Log("yes");
        
    }

    void b()
    {
        Managers.Instance.UIManager.ClosePopupUI();
        Debug.Log("no");
        
    }

    public double outp = 0;
    private void Update()
    {
        outp = Managers.Instance.GameManager.GetTotalPlayTime();
    }

    public double total = 0;

    [ContextMenu("ini")]
    void inI()
    {
        Managers.Instance.GameManager.InitTotalPlayTimeChecker(total);
    }

    public string tex="asd";
    [ContextMenu("alert")]
    void tetsaf()
    {
        Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText(tex);
    }



    CooldownComponentModule timer;

    [ContextMenu("timer")]
    void tim()
    {
        timer = Managers.Instance.CooldownManager.GetCooldownModule(5);

        timer.OnCooldownEnded += () => { Debug.Log("end"); Managers.Instance.CooldownManager.ReturnModule(timer); };

        timer.StartCooldown();


    }

}
