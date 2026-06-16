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
            Debug.Log($"{name} init complete");
        }
    }
}