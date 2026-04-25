using UnityEngine;
using Defines;

namespace Scenes
{
    public class SceneBase : MonoBehaviour
    {
        public SceneType NowSceneType { get; protected set; } = SceneType.UNKNOWN;//이거 방식 바꾸기

        private void Awake()
        {
            Init();
        }
        protected virtual void Init()
        {

        }
    }
}
