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
            _renderer = GetComponent<Renderer>();
            _animator = GetComponent<Animator>();
        }

        public void Init(bool initialState)
        {
            SetState(initialState);
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
            }
            _state = state;
        }

        public void UnregisterOnStateChanged(Action<bool> callback)
        {
            _onStateChanged -= callback;
        }
    }
}