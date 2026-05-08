using Defines;
using Data;
using UnityEngine;
using InputHandler;
using Coordinator.Movements;
using ComponentModule;
using Manager;
using System;

namespace Coordinator
{
    public abstract class ModeBase : MonoBehaviour,ILMBInputHandler, IRMBInputHandler, IDropInputHandler, IStunnable
    {
        protected PlayerInputCoordinator _inputCoordinator;
        protected CommonModeData _commonData;

        protected CooldownComponentModule _stunCounter;
        protected Action _onStunEnd;
        protected bool _isStunned;
        public bool IsUnlocked { get; set; } = false;
        public virtual ModeTypes ModeType { get; }

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _inputCoordinator = gameObject.GetComponentInParent<PlayerInputCoordinator>();
            _onStunEnd = OnStunEnd;
        }

        public virtual void Init(CommonModeData data)
        {
            gameObject.SetActive(true);
            _inputCoordinator.Init();
            _commonData = data;
            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);
            _isStunned = false;
            if(_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
            _stunCounter = Managers.Instance.CooldownManager.GetCooldownModule(0);
            _stunCounter.OnCooldownEnded += _onStunEnd;
        }

        public virtual void DeInit()
        {
            if(_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
            gameObject.SetActive(false);
        }

        public CommonModeData GetSharedData()
        {
            return _commonData;
        }

        public abstract void OnRMBEvent(bool pressed, Vector2 screenPos);

        public abstract void OnDropEvent(bool pressed);

        public abstract void OnLMBEvent(bool pressed, Vector2 screenPos);
        protected virtual void OnStunEnd() { }

        public abstract void StunFor(float time);
        public abstract void ReleaseStun();
    }
}