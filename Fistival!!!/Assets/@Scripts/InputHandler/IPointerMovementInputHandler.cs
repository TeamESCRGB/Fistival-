using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IPointerMovementInputHandler
    {
        public void OnPointerMove(InputAction.CallbackContext callbackContext);
    }
}
