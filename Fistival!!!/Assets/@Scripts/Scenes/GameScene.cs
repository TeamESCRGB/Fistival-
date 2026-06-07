using Defines;
using UnityEngine;

namespace Scenes
{
    public class GameScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.GameScene;

        protected override void Init()
        {
            base.Init();

            Debug.Log($"{name} init complete");
        }
    }
}