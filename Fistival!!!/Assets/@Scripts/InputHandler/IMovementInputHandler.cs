using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IMovementInputHandler
    {
        public void OnLeftMovementInputEvent(bool pressed);
        public void OnRightMovementInputEvent(bool pressed);
        public void OnJumpMovementInputEvent(bool pressed);
        public void OnDownMovementInputEvent(bool pressed);
    }
}