using Coordinator;
using Defines;
using Manager;
using UI;
using UnityEngine;

namespace Scenes
{
    public class LobbyScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.LobbyScene;

        private PlayerHUD _hud;

        protected override void Init()
        {
            base.Init();

            Managers.Instance.GameManager.InitPauseState();
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.PLAYER);

            _hud = FindAnyObjectByType<PlayerHUD>();
            _hud.gameObject.SetActive(false);
            Managers.Instance.StageManager.Init();
            Debug.Log($"{name} init complete");
        }

        private void Start()
        {
            var player = FindAnyObjectByType<PlayerCoordinator>();
            if(player != null)
            {
                player.Init();
            }
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, "LobbyBGM", true, Managers.Instance.GameManager.BGMVolume);
            _hud.gameObject.SetActive(true);
            //var modeManageCoord = FindAnyObjectByType<ModeManageCoordinator>();
            //if(modeManageCoord != null)
            //{
            //    modeManageCoord.UnlockMode(ModeTypes.FISTIVAL);
            //    modeManageCoord.ChangeMode(ModeTypes.FISTIVAL);
            //}
        }
    }
}