using Defines;
using Manager;
using UnityEngine;

namespace Scenes
{
    public class MainScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.MainScene;

        protected override void Init()
        {
            base.Init();
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.UI);
            Managers.Instance.UIManager.DIsableAutoUIActionMapChange();
            Debug.Log($"{name} init complete");
        }

        private void Start()
        {
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, "MainBGM", true);
        }
    }
}