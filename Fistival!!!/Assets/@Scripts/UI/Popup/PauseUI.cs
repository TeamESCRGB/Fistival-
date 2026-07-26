using Defines;
using Manager;
using System.Collections;
using UI.Transition;
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

        enum Texts
        {
            VersionText
        }

        private bool _isMainSceneChangeTriggered = false;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            if(Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType != Defines.SceneType.GameScene)
            {
                GetButton((int)Buttons.TO_LOBBY).gameObject.SetActive(false);
            }

            GetButton((int)Buttons.TO_MAIN).gameObject.BindUIEvent(OnToMain);
            GetButton((int)Buttons.TO_LOBBY).gameObject.BindUIEvent(OnToLobby);
            GetButton((int)Buttons.SETTING).gameObject.BindUIEvent(OnSetting);
            GetButton((int)Buttons.QUIT_GAME).gameObject.BindUIEvent(OnQuitGame);
            GetButton((int)Buttons.RESUME).gameObject.BindUIEvent(OnResume);

            GetText((int)Texts.VersionText).text = $"v{Application.version}";

            return true;
        }

        private void OnToMain(PointerEventData _)
        {
            if(_isMainSceneChangeTriggered == false)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnMainMenuYes, OnConfirmNo).SetText("메인 화면으로 돌아가시겠습니까?\n저장되지 않은 모든 정보는 삭제됩니다!");
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            }
        }

        private void OnToLobby(PointerEventData _)
        {
#if DISABLE_LOBBY_SCENE
            Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("지금은 이용할 수 없는 기능입니다.");
#else
            if(_isMainSceneChangeTriggered == false)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLobbyYes, OnConfirmNo).SetText("로비 화면으로 돌아가시겠습니까?\n저장되지 않은 모든 진행상황은 삭제됩니다!");
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            }
#endif
        }

        private void OnQuitGame(PointerEventData _)
        {
            if(_isMainSceneChangeTriggered == false)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnQuitGameYes, OnConfirmNo).SetText("게임을 종료하시겠습니까? 저장되지 않은 모든 정보는 삭제됩니다!");
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            }
        }

        private void OnSetting(PointerEventData _)
        {
            if(_isMainSceneChangeTriggered == false)
            {
                Managers.Instance.UIManager.ShowPopupUI<SettingMenu>("SettingMenu");
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            }
        }

        private void OnResume(PointerEventData _)
        {
            if(_isMainSceneChangeTriggered == false)
            {
                Managers.Instance.GameManager.UnPauseGame();
                Managers.Instance.UIManager.ClosePopupUI();
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            }
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
            if(_isMainSceneChangeTriggered || ctx.started || ctx.control.IsPressed() == false)
            {
                return;
            }

            if(Managers.Instance.UIManager.CompareTopPopup(this) == false)
            {
                Debug.Log("fal");
                return;
            }
            
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.GameManager.UnPauseGame();
        }

        private void OnQuitGameYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
        }

        private void OnMainMenuYes()
        {
            _isMainSceneChangeTriggered = true;
            StartCoroutine(LoadMainScene());
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
        }

        private IEnumerator LoadMainScene()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            GetComponentInChildren<BookFlip>().Close(1);
            yield return new WaitForSecondsRealtime(1);

            var nowScene = Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType;

            Managers.Instance.ResourceManager.LoadAsyncAllIn("MainSceneLoaded", (_, now, end) =>
            {
                if (now < end)
                {
                    return;
                }

                Managers.Instance.ResourceManager.ReleaseIn("LobbySceneLoaded");
                Managers.Instance.ResourceManager.ReleaseIn("GameSceneBasicLoaded");
                Managers.Instance.StageManager.Init();
                Managers.Instance.SceneManagerEx.LoadScene(SceneType.MainScene);
            });
        }

        private void OnLobbyYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            var nowScene = Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType;
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
            Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (_, now, end) =>
            {
                if (now < end)
                {
                    return;
                }

                Managers.Instance.ResourceManager.ReleaseIn("GameSceneBasicLoaded");
                Managers.Instance.StageManager.Init();

                Managers.Instance.SceneManagerEx.LoadScene(SceneType.LobbyScene);
            });
        }


        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false, Managers.Instance.GameManager.SFXVolume);
        }
    }
}