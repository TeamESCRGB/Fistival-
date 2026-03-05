using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IRMBInputHandler
    {
        public void OnRMBEvent(InputAction.CallbackContext callbackContext);
    }
}
