using UnityEngine;
using UnityEngine.InputSystem;

namespace InputHandler
{
    public interface IPointerMovementInputHandler
    {
        public void OnPointerMove(Vector2 screenPos);
    }
}
