using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IDropInputHandler
    {
        public void OnDropEvent(bool pressed);
    }
}