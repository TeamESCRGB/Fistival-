using Manager;
using UI.Transition;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class CreditPopup : UIPopupBase
    {
        enum Buttons
        {
            ReturnToMain
        }
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.ReturnToMain).gameObject.BindUIEvent(OnReturnToLobby);

            return true;
        }

        private void OnReturnToLobby(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();

            var nowScene = Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType;

            Managers.Instance.ResourceManager.LoadAsyncAllIn("MainSceneLoaded", (_, now, end) =>
            {
                if (now < end)
                {
                    return;
                }

                Managers.Instance.StageManager.Init();
                Managers.Instance.ResourceManager.ReleaseIn("LobbySceneLoaded");
                Managers.Instance.ResourceManager.ReleaseIn("GameSceneBasicLoaded");

                Managers.Instance.SceneManagerEx.LoadScene(Defines.SceneType.MainScene);
            });
        }
    }
}