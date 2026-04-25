using UnityEngine;
using Defines;

namespace Scenes
{
    public class SceneBase : MonoBehaviour
    {
        public virtual SceneType NowSceneType { get; }

        private void Awake()
        {
            Init();
        }
        protected virtual void Init()
        {

        }
    }
}
