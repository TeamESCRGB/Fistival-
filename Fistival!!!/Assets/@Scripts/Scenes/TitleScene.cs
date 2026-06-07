using Defines;
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
    }
}