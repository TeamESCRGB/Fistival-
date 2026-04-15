using Defines;
using Data;
using UnityEngine;

namespace Coordinator
{
    public abstract class ModeBase : MonoBehaviour
    {
        protected PlayerInputCoordinator _inputCoordinator;
        protected CommonModeData _commonData;
        public bool IsUnlocked { get; set; } = false;
        public virtual ModeTypes ModeType { get; }

        public virtual void Init(CommonModeData data)
        {
            gameObject.SetActive(true);
            _inputCoordinator.Init();
            _commonData = data;

        }

        public virtual void DeInit()
        {
            gameObject.SetActive(false);
        }

        public CommonModeData GetSharedData()
        {
            return _commonData;
        }
    }
}