using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface ILMBInputHandler
    {
        public void OnLMBEvent(InputAction.CallbackContext callbackContext);
    }
}