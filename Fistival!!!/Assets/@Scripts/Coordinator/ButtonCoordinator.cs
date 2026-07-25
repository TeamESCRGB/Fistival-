using Manager;
using System;
using UnityEngine;

namespace Coordinator
{
    public class ButtonCoordinator : InteractableObjectCoordinator, IToggle
    {
        private bool _state;
        private event Action<bool> _onStateChanged;
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Init(bool initialState)
        {
            SetState_forInit(initialState);
            _animator.SetBool("IsOn", initialState);
        }
        [ContextMenu("init")]
        void d()
        {
            Init(false);
        }
        [ContextMenu("off")]
        void f()
        {
            SetState(false);
        }

        public override void Interact()
        {
            SetState(true);
        }

        public bool GetState()
        {
            return _state;
        }

        public void RegisterOnStateChanged(Action<bool> callback)
        {
            _onStateChanged -= callback;
            _onStateChanged += callback;
        }

        public void SetState(bool state)
        {
            if(_state != state)
            {
                _onStateChanged?.Invoke(state);
                _animator.SetBool("IsOn",state);
                if (state)
                {
                    Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "Button", false, Managers.Instance.GameManager.SFXVolume);
                }
            }

            FixHighlightState(state);

            _state = state;
        }

        protected void SetState_forInit(bool state)
        {
            if (_state != state)
            {
                _onStateChanged?.Invoke(state);
                _animator.SetBool("IsOn", state);
            }
            FixHighlightState(state);
            _state = state;
        }

        public void UnregisterOnStateChanged(Action<bool> callback)
        {
            _onStateChanged -= callback;
        }
    }
}