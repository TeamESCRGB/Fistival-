using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IMovementInputHandler
    {
        public void OnLeftMovementInputEvent(InputAction.CallbackContext callbackContext);
        public void OnRightMovementInputEvent(InputAction.CallbackContext callbackContext);
        public void OnJumpMovementInputEvent(InputAction.CallbackContext callbackContext);
        public void OnDownMovementInputEvent(InputAction.CallbackContext callbackContext);
    }
}