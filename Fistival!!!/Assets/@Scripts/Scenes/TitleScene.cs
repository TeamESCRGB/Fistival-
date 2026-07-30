using Defines;
using Manager;
using UnityEngine;

namespace Scenes
{
    public class TitleScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.TitleScene;

        protected override void Init()
        {
            base.Init();
            
            Debug.Log($"{name} init complete");
        }

        private void Start()
        {
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.UI);
            Managers.Instance.UIManager.DIsableAutoUIActionMapChange();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, "TitleBGM", true);
        }
    }
}