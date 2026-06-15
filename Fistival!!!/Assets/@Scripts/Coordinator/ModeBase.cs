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
    public abstract class ModeBase : MonoBehaviour,ILMBInputHandler, IRMBInputHandler, IDropInputHandler, IStunnable, IESCInputHandler, IInteractionInputHandler
    {
        protected PlayerInputCoordinator _inputCoordinator;
        protected CommonModeData _commonData;
        protected PlayerData _playerData;
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
            _playerData = GetComponentInParent<PlayerCoordinator>().GetPlayerData();
            _inputCoordinator.Init();
            _commonData = data;
            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);
            _inputCoordinator.SetESCInputHandler(this);
            _inputCoordinator.SetInteractionInputHandler(this);
            _isStunned = false;
            if(_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
            _stunCounter = Managers.Instance.CooldownManager.GetCooldownModule(0);
            _stunCounter.OnCooldownEnded += _onStunEnd;

            Managers.Instance.NewInputSystemManager.OnActionMapChanged -= OnInputActionMapChanged;
            Managers.Instance.NewInputSystemManager.OnActionMapChanged += OnInputActionMapChanged;
        }

        public virtual void DeInit()
        {
            if(_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
            Managers.Instance.NewInputSystemManager.OnActionMapChanged -= OnInputActionMapChanged;
            gameObject.SetActive(false);
        }

        public abstract void UpdateUpdatedPlayerData();

        protected virtual void OnInputActionMapChanged(ActionMapTypes mapType)
        {
            if(mapType != ActionMapTypes.PLAYER)
            {
                _inputCoordinator.TriggerReleaseMovementInput();
            }
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

        public void OnESCInputEvent(bool pressed)
        {
            Managers.Instance.GameManager.PauseGame();
        }

        public abstract void OnInteractionInputEvent(bool pressed);
    }
}