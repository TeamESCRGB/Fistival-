using Defines;
using Data;
using UnityEngine;
using InputHandler;
using Coordinator.Movements;
using ComponentModule;
using Manager;
using System;
using Coordinator.Victims;
using Data.NonLodable;

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
        protected Animator _animator;
        protected Rigidbody2D _rb2d;
        protected MovementSoundKeys _movementSoundKeys;
        public bool IsUnlocked { get; set; } = false;
        public virtual ModeTypes ModeType { get; }

        private void Awake()
        {
            OnAwake();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnAwake()
        {
            _inputCoordinator = gameObject.GetComponentInParent<PlayerInputCoordinator>();
            _animator = gameObject.GetComponentInParent<Animator>();
            _onStunEnd = OnStunEnd;
            _rb2d = GetComponentInParent<Rigidbody2D>();
        }

        protected virtual void OnFixedUpdate()
        {
            _animator.SetFloat("YVelocity", _rb2d.linearVelocityY);
        }

        public void PreInit(CommonModeData data, bool changedFromOther)
        {
            gameObject.SetActive(true);
            _commonData = data;
            var player = GetComponentInParent<PlayerCoordinator>();
            player.GetComponentInChildren<PlayerVictimCoordinator>().SetAttackableState(false);
            _playerData = player.GetPlayerData();
            _movementSoundKeys = _playerData.MovementSoundKeysField;
            _inputCoordinator.TriggerReleaseMovementInput();
            _inputCoordinator.Init();
            _inputCoordinator.SetESCInputHandler(this);
            _animator.runtimeAnimatorController = Managers.Instance.ResourceManager.Load<RuntimeAnimatorController>(data.AnimControllerName);
            if(changedFromOther)
            {
                _animator.SetTrigger("EnterMode");
            }
        }

        public void PreDeInit()
        {
            _inputCoordinator.TriggerReleaseMovementInput();
            _inputCoordinator.Init();
            _inputCoordinator.SetESCInputHandler(this);
            if (_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
            _animator.SetTrigger("ExitMode");
        }

        public virtual void Init(CommonModeData data)
        {
            var player = GetComponentInParent<PlayerCoordinator>();
            player.GetComponentInChildren<PlayerVictimCoordinator>().SetAttackableState(true);
            _isStunned = false;
            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);
            _inputCoordinator.SetInteractionInputHandler(this);
            if (_stunCounter is not null)
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