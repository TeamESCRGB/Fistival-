using Defines;
using UnityEngine;

namespace Scenes
{
    public class LobbyScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.LobbyScene;

        protected override void Init()
        {
            base.Init();

            Debug.Log($"{name} init complete");
        }
    }
}