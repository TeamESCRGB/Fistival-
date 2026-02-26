using UnityEngine;
using Defines;

namespace Scenes
{
    public class SceneBase : MonoBehaviour
    {
        public SceneType NowSceneType { get; protected set; } = SceneType.UNKNOWN;

        private void Awake()
        {
            Init();
        }
        protected void Init()
        {

        }
    }
}
