using Defines;
using Data;
using UnityEngine;
using InputHandler;

namespace Coordinator
{
    public abstract class ModeBase : MonoBehaviour,ILMBInputHandler, IRMBInputHandler, IDropInputHandler
    {
        protected PlayerInputCoordinator _inputCoordinator;
        protected CommonModeData _commonData;
        public bool IsUnlocked { get; set; } = false;
        public virtual ModeTypes ModeType { get; }

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _inputCoordinator = gameObject.GetComponentInParent<PlayerInputCoordinator>();
        }

        public virtual void Init(CommonModeData data)
        {
            gameObject.SetActive(true);
            _inputCoordinator.Init();
            _commonData = data;
            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);

        }

        public virtual void DeInit()
        {
            gameObject.SetActive(false);
        }

        public CommonModeData GetSharedData()
        {
            return _commonData;
        }

        public abstract void OnRMBEvent(bool pressed, Vector2 screenPos);

        public abstract void OnDropEvent(bool pressed);

        public abstract void OnLMBEvent(bool pressed, Vector2 screenPos);
    }
}