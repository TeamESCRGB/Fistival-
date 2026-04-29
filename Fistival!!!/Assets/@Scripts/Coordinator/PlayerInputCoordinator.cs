using InputHandler;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Coordinator
{
    public class PlayerInputCoordinator : MonoBehaviour
    {
        private bool _isDownJumpTriggered = false;
        private ILMBInputHandler _lmbHandler;
        private IRMBInputHandler _rmbHandler;
        private IPointerMovementInputHandler _pointerHandler;
        private IDropInputHandler _dropHandler;
        private IReloadInputHandler _reloadHandler;

        private IJumpsMovementInputHandler _jumpsHandler;
        private IHorizontalMovementInputHandler _horizontalMovementHandler;
        private IVerticalMovementInputHandler _verticalMovementHandler;

        private Vector2 _lastPos;

        public void SetLMBInputHandler(ILMBInputHandler handler)
        {
            _lmbHandler=handler;
        }
        public void SetRMBInputHandler(IRMBInputHandler handler)
        {
            _rmbHandler=handler;
        }
        public void SetPointerMovementInputHandler(IPointerMovementInputHandler handler)
        {
            _pointerHandler=handler;
        }

        public void SetDropInputHandler(IDropInputHandler handler)
        {
            _dropHandler = handler;
        }


        public void SetReloadInputHandler(IReloadInputHandler handler)
        {
            _reloadHandler = handler;
        }


        #region Movement
        public void SetJumpsMovementInputHandler(IJumpsMovementInputHandler handler)
        {
            _jumpsHandler=handler;
        }

        public void SetVerticalMovementInputHandler(IVerticalMovementInputHandler handler)
        {
            _verticalMovementHandler = handler;
        }

        public void SetHorizontalMovementInputHandler(IHorizontalMovementInputHandler handler)
        {
            _horizontalMovementHandler=handler;
        }
        #endregion


        public void Init()
        {
            _jumpsHandler = null;
            _verticalMovementHandler =null;
            _horizontalMovementHandler = null;

            _reloadHandler = null;
            _isDownJumpTriggered = false;
            _lmbHandler = null;
            _rmbHandler = null;
            _pointerHandler = null;
            _dropHandler = null;
            _lastPos = Vector2.zero;
        }

        public void OnReloadInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _reloadHandler?.OnReloadInputEvent(callbackContext.control.IsPressed());
        }

        public void OnUpMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }
            _verticalMovementHandler?.OnUpMovementInputEvent(callbackContext.control.IsPressed());
        }

        public void OnDownMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            if (callbackContext.control.IsPressed())
            {
                _verticalMovementHandler?.OnDownMovementInputEvent(true);
                _isDownJumpTriggered = true;
            }
            else
            {
                _verticalMovementHandler?.OnDownMovementInputEvent(false);
                _isDownJumpTriggered = false;
            }
        }

        public void OnJumpMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }

            if(_isDownJumpTriggered)
            {
                _jumpsHandler?.OnDownJumpMovementInputEvent(callbackContext.control.IsPressed());
            }
            else
            {
                _jumpsHandler?.OnJumpMovementInputEvent(callbackContext.control.IsPressed());
            }
        }

        public void OnLeftMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _horizontalMovementHandler?.OnLeftMovementInputEvent(callbackContext.control.IsPressed());
        }
        public void OnRightMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _horizontalMovementHandler?.OnRightMovementInputEvent(callbackContext.control.IsPressed());
        }

        public void OnPointerMove(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _lastPos = callbackContext.ReadValue<Vector2>();
            _pointerHandler?.OnPointerMove(_lastPos);
        }
        public void OnLMBEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _lmbHandler?.OnLMBEvent(callbackContext.control.IsPressed(), _lastPos);
        }
        public void OnRMBEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _rmbHandler?.OnRMBEvent(callbackContext.control.IsPressed(), _lastPos);
        }

        public void OnDropInput(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }
            _dropHandler?.OnDropEvent(callbackContext.control.IsPressed());
        }
    }
}
