using Defines;
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

        enum Texts
        {
            VersionText
        }

        private BookFlipController _bfc;
#if DISABLE_LOBBY_SCENE
        [SerializeField]
        private int _stageIdx=0;
#endif
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            _bfc = GetComponent<BookFlipController>();
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            GetButton((int)Buttons.NewGame).gameObject.BindUIEvent(OnNewGame);
            GetButton((int)Buttons.LoadGame).gameObject.BindUIEvent(OnLoadGame);

            GetButton((int)Buttons.Setting).gameObject.BindUIEvent(OnSetting);
            GetButton((int)Buttons.QuitGame).gameObject.BindUIEvent(OnQuitGame);

            GetButton((int)Buttons.NewGame).GetComponent<RectTransform>().DOAnchorPosX(50,1).SetDelay(0.25f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.LoadGame).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(0.5f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.Setting).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(0.75f).SetEase(Ease.OutBack);
            GetButton((int)Buttons.QuitGame).GetComponent<RectTransform>().DOAnchorPosX(50, 1).SetDelay(1).SetEase(Ease.OutBack);
            GetText((int)Texts.VersionText).text = $"v{Application.version}";

            return true;
        }

        private void OnNewGame(PointerEventData data)
        {
#if DISABLE_LOBBY_SCENE
            Managers.Instance.ResourceManager.LoadAsyncAllIn("GameSceneBasicLoaded", (_, gameNow, gameMax) =>
            {
                if (gameNow < gameMax)
                {
                    return;
                }

                Managers.Instance.StageManager.SetStageIDX(_stageIdx);

                string loadKey = Managers.Instance.StageManager.GetStageData()?.FirstStageLoadedDatasName;
                loadKey = loadKey is null ? "" : loadKey;

                Managers.Instance.ResourceManager.LoadAsyncAllIn(loadKey, (_, now, max) =>
                {
                    if (now == max)
                    {
                        Managers.Instance.ResourceManager.ReleaseIn("LobbySceneLoaded");
                        Managers.Instance.SceneManagerEx.LoadScene(SceneType.GameScene);
                    }
                });
            });
#else
            GetComponentInChildren<GameFileMenu>().Open();
            _bfc.FlipTo(0);
#endif
        }

        private void OnLoadGame(PointerEventData data)
        {
#if DISABLE_LOBBY_SCENE
            Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("지금은 이용할 수 없는 기능입니다.");
#else
            _bfc.FlipTo(1);
            GetComponentInChildren<SaveFileMenu>().SetMenuType(Defines.SaveFileAccessMode.LOAD);
#endif
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
