using InputHandler;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Coordinator
{
    public class PlayerInputCoordinator : MonoBehaviour
    {
        private bool _isDownTriggered = false;
        private ILMBInputHandler _lmbHandler;
        private IRMBInputHandler _rmbHandler;
        private IPointerMovementInputHandler _pointerHandler;
        private IMovementInputHandler _movementHandler;
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
        public void SetMovementInputHandler(IMovementInputHandler handler)
        {
            _movementHandler=handler;
        }

        public void OnDownMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            if (callbackContext.control.IsPressed())
            {
                _isDownTriggered = true;
            }
            else
            {
                _isDownTriggered = false;
            }
        }

        public void OnJumpMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }

            if(_isDownTriggered)
            {
                _movementHandler?.OnDownMovementInputEvent(callbackContext.control.IsPressed());
            }
            else
            {
                _movementHandler?.OnJumpMovementInputEvent(callbackContext.control.IsPressed());
            }
        }

        public void OnLeftMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _movementHandler?.OnLeftMovementInputEvent(callbackContext.control.IsPressed());
        }
        public void OnRightMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _movementHandler?.OnRightMovementInputEvent(callbackContext.control.IsPressed());
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
    }
}
