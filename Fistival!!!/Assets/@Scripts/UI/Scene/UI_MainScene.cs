using DG.Tweening;
using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Popup;
using UI.Transition;
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
        private BookFlipController _bfc;
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            _bfc = GetComponent<BookFlipController>();
            BindButton(typeof(Buttons));
            GetButton((int)Buttons.NewGame).gameObject.BindUIEvent(OnNewGame);
            GetButton((int)Buttons.LoadGame).gameObject.BindUIEvent(OnLoadGame);
            GetButton((int)Buttons.Setting).gameObject.BindUIEvent(OnSetting);
            GetButton((int)Buttons.QuitGame).gameObject.BindUIEvent(OnQuitGame);

            GetButton((int)Buttons.NewGame).GetComponent<RectTransform>().DOAnchorPosX(50,1).SetDelay(0.25f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.LoadGame).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(0.5f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.Setting).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(0.75f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.QuitGame).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(1).SetEase(Ease.OutBack);


            return true;
        }

        private void OnNewGame(PointerEventData data)
        {
            //Managers.Instance.UIManager.ShowPopupUI<GameFileMenu>("GameFileMenu");
            GetComponentInChildren<GameFileMenu>().Open();
            _bfc.FlipTo(0);
        }

        private void OnLoadGame(PointerEventData data)
        {
            //Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(Defines.SaveFileAccessMode.LOAD);
            _bfc.FlipTo(1);
            GetComponentInChildren<SaveFileMenu>().SetMenuType(Defines.SaveFileAccessMode.LOAD);
        }

        private void OnSetting(PointerEventData data)
        {
            //Managers.Instance.UIManager.ShowPopupUI<SettingMenu>("SettingMenu");
            _bfc.FlipTo(2);
            GetComponentInChildren<SettingMenu>();
        }

        private void OnQuitGame(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(QuitGame,CloseConfirm).SetText("게임을 종료하시겠습니까?");
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
