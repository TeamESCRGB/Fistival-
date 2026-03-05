using UnityEngine;
using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface ILMBInputHandler
    {
        public void OnLMBEvent(bool pressed, Vector2 screenPos);
    }
}