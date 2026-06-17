using Defines;
using Manager;
using UnityEngine;

namespace Scenes
{
    public class GameScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.GameScene;

        protected override void Init()
        {
            base.Init();
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.PLAYER);
            Debug.Log($"{name} init complete");
        }
    }
}