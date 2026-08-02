//using Data.NonLoader;
using Data.NonLodable;
using Defines;
using Manager;
using Manager.Contents;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI.Popup;
using UnityEngine;
using Utils;

public class TestSaveFile : MonoBehaviour
{

    [ContextMenu("f")]
    void Save()
    {
        Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(SaveFileAccessMode.OVERWRITE);
    }

    [ContextMenu("fㅁ")]
    void load()
    {
        Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(SaveFileAccessMode.LOAD);
    }

    public double time;

    private void Update()
    {
        time = Managers.Instance.GameManager.GetTotalPlayTime();
    }
}
