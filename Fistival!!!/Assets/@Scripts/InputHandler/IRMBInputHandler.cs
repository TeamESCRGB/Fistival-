using UnityEngine;
using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IRMBInputHandler
    {
        public void OnRMBEvent(bool pressed, Vector2 screenPos);
    }
}
