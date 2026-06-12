using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;

namespace UI.Popup
{
    public class PauseUI : UIPopupBase
    {
        enum Buttons
        {
            TO_MAIN,
            TO_LOBBY,
            SETTING,
            QUIT_GAME,
            RESUME
        }
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));

            if(Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType != Defines.SceneType.GameScene)
            {
                GetButton((int)Buttons.TO_LOBBY).gameObject.SetActive(false);
            }

            GetButton((int)Buttons.TO_MAIN).gameObject.BindUIEvent(OnToMain);
            GetButton((int)Buttons.TO_LOBBY).gameObject.BindUIEvent(OnToLobby);
            GetButton((int)Buttons.SETTING).gameObject.BindUIEvent(OnSetting);
            GetButton((int)Buttons.QUIT_GAME).gameObject.BindUIEvent(OnQuitGame);
            GetButton((int)Buttons.RESUME).gameObject.BindUIEvent(OnResume);

            return true;
        }

        private void OnToMain(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnMainMenuYes, OnConfirmNo);
        }

        private void OnToLobby(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLobbyYes, OnConfirmNo);
        }

        private void OnQuitGame(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnQuitGameYes, OnConfirmNo);
        }

        private void OnSetting(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<SettingMenu>("SettingMenu");
        }

        private void OnResume(PointerEventData _)
        {
            Managers.Instance.GameManager.UnPauseGame();
            Managers.Instance.UIManager.ClosePopupUI();
        }


        protected override void Start()
        {
            base.Start();
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= OnESCInput;
            Managers.Instance.NewInputSystemManager.UI_ESCInput += OnESCInput;
        }

        private void OnDisable()
        {
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= OnESCInput;
        }

        private void OnESCInput(InputAction.CallbackContext ctx)
        {
            if(ctx.started || ctx.control.IsPressed() == false)
            {
                return;
            }

            if(Managers.Instance.UIManager.CompareTopPopup(this) == false)
            {
                Debug.Log("fal");
                return;
            }
            Managers.Instance.GameManager.UnPauseGame();
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnQuitGameYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Debug.Log("게임종료");
        }

        private void OnMainMenuYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Debug.Log("메인메뉴");
        }

        private void OnLobbyYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Debug.Log("로비");
        }


        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}