using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Popup;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    internal class TestTalk : MonoBehaviour
    {
        [ContextMenu("start")]
        public void asdf()
        {
            Managers.Instance.UIManager.ShowPopupUI<TalkCutScenePopup>("TalkCutScenePopup").SetData("TestTalk").SetOnEnd(() => { Debug.Log("asdf"); });
        }
    }
}
