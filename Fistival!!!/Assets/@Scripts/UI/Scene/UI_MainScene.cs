using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Scene
{
    public class UI_MainScene : UISceneBase
    {
        enum Buttons
        {
            NewGame,
            LoadGame,
            Setting,
            QuitGame
        }
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            GetButton((int)Buttons.NewGame).gameObject.BindUIEvent(OnNewGame);
            GetButton((int)Buttons.LoadGame).gameObject.BindUIEvent(OnLoadGame);
            GetButton((int)Buttons.Setting).gameObject.BindUIEvent(OnSetting);
            GetButton((int)Buttons.QuitGame).gameObject.BindUIEvent(OnQuitGame);

            return true;
        }

        private void OnNewGame(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<GameFileMenu>("GameFileMenu");
        }

        private void OnLoadGame(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(Defines.SaveFileAccessMode.LOAD);
        }

        private void OnSetting(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<SettingMenu>("SettingMenu");
        }

        private void OnQuitGame(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(QuitGame,CloseConfirm);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void CloseConfirm()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}
