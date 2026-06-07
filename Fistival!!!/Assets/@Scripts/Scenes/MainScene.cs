using Defines;
using UnityEngine;

namespace Scenes
{
    public class MainScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.MainScene;

        protected override void Init()
        {
            base.Init();

            Debug.Log($"{name} init complete");
        }
    }
}