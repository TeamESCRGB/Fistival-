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
            GetButton((int)Buttons.RESUME).gameObject.BindUIEvent(OnResume);

            return true;
        }

        private void OnToMain(PointerEventData _)
        {
            Debug.Log("메인 클릭");
        }

        private void OnToLobby(PointerEventData _)
        {
            Debug.Log("로비 클릭");
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
            Managers.Instance.GameManager.UnPauseGame();
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}