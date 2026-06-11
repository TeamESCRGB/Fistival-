using Defines;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager.Core
{
    public class NewInputSystemManager : MonoBehaviour
    {
        private PlayerInput _input;
        public Action<ActionMapTypes> OnActionMapChanged;
        public event Action<InputAction.CallbackContext> Player_RMBInput;
        public event Action<InputAction.CallbackContext> Player_LMBInput;
        public event Action<InputAction.CallbackContext> Player_PointerMovementInput;
        public event Action<InputAction.CallbackContext> Player_LeftMovementInput;
        public event Action<InputAction.CallbackContext> Player_RightMovementInput;
        public event Action<InputAction.CallbackContext> Player_DownTrigger_for_bug_fix;
        public event Action<InputAction.CallbackContext> Player_JumpInput;
        public event Action<InputAction.CallbackContext> Player_DropInput;
        public event Action<InputAction.CallbackContext> Player_DiagnosticsInput;
        public event Action<InputAction.CallbackContext> Player_UpMovementInput;
        public event Action<InputAction.CallbackContext> Player_ReloadInput;
        public event Action<InputAction.CallbackContext> Player_InteractionInput;
        public event Action<InputAction.CallbackContext> Player_ESCInput;
        public event Action<InputAction.CallbackContext> UI_Navigate;
        public event Action<InputAction.CallbackContext> UI_Submit;
        public event Action<InputAction.CallbackContext> UI_Cancel;
        public event Action<InputAction.CallbackContext> UI_Point;
        public event Action<InputAction.CallbackContext> UI_Click;
        public event Action<InputAction.CallbackContext> UI_RightClick;
        public event Action<InputAction.CallbackContext> UI_MiddleClick;
        public event Action<InputAction.CallbackContext> UI_ScrollWheel;
        public event Action<InputAction.CallbackContext> UI_TrackedDevicePosition;
        public event Action<InputAction.CallbackContext> UI_TrackedDeviceOrientation;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
#if UNITY_EDITOR
            Debug.Assert(_input != null, $"{name}에 PlayerInput이 없습니다");
#endif
        }

        public void SwitchActionMap(ActionMapTypes actionMap)
        {
            _input.SwitchCurrentActionMap(actionMap.ToString());
            OnActionMapChanged?.Invoke(actionMap);
        }

        public void Clear()
        {
            OnActionMapChanged = null;
            Player_RMBInput = null;
            Player_LMBInput = null;
            Player_PointerMovementInput = null;
            Player_LeftMovementInput = null;
            Player_RightMovementInput = null;
            Player_DownTrigger_for_bug_fix = null;
            Player_JumpInput = null;
            Player_DropInput = null;
            Player_DiagnosticsInput = null;
            Player_UpMovementInput = null;
            Player_ReloadInput = null;
            Player_InteractionInput = null;
            Player_ESCInput = null;
            UI_Navigate = null;
            UI_Submit = null;
            UI_Cancel = null;
            UI_Point = null;
            UI_Click = null;
            UI_RightClick = null;
            UI_MiddleClick = null;
            UI_ScrollWheel = null;
            UI_TrackedDevicePosition = null;
            UI_TrackedDeviceOrientation = null;
        }

        #region Player
        public void OnPlayer_RMBInput(InputAction.CallbackContext ctx)
        {
            Player_RMBInput?.Invoke(ctx);
        }

        public void OnPlayer_LMBInput(InputAction.CallbackContext ctx)
        {
            Player_LMBInput?.Invoke(ctx);
        }

        public void OnPlayer_PointerMovementInput(InputAction.CallbackContext ctx)
        {
            Player_PointerMovementInput?.Invoke(ctx);
        }

        public void OnPlayer_LeftMovementInput(InputAction.CallbackContext ctx)
        {
            Player_LeftMovementInput?.Invoke(ctx);
        }

        public void OnPlayer_RightMovementInput(InputAction.CallbackContext ctx)
        {
            Player_RightMovementInput?.Invoke(ctx);
        }

        public void OnPlayer_DownTrigger_for_bug_fix(InputAction.CallbackContext ctx)
        {
            Player_DownTrigger_for_bug_fix?.Invoke(ctx);
        }

        public void OnPlayer_JumpInput(InputAction.CallbackContext ctx)
        {
            Player_JumpInput?.Invoke(ctx);
        }

        public void OnPlayer_DropInput(InputAction.CallbackContext ctx)
        {
            Player_DropInput?.Invoke(ctx);
        }

        public void OnPlayer_DiagnosticsInput(InputAction.CallbackContext ctx)
        {
            Player_DiagnosticsInput?.Invoke(ctx);
        }

        public void OnPlayer_UpMovementInput(InputAction.CallbackContext ctx)
        {
            Player_UpMovementInput?.Invoke(ctx);
        }

        public void OnPlayer_ReloadInput(InputAction.CallbackContext ctx)
        {
            Player_ReloadInput?.Invoke(ctx);
        }

        public void OnPlayer_InteractionInput(InputAction.CallbackContext ctx)
        {
            Player_InteractionInput?.Invoke(ctx);
        }

        public void OnPlayer_ESCInput(InputAction.CallbackContext ctx)
        {
            Player_ESCInput?.Invoke(ctx);
        }

        #endregion

        #region UI
        public void OnUI_Navigate(InputAction.CallbackContext ctx)
        {
            UI_Navigate?.Invoke(ctx);
        }

        public void OnUI_Submit(InputAction.CallbackContext ctx)
        {
            UI_Submit?.Invoke(ctx);
        }

        public void OnUI_Cancel(InputAction.CallbackContext ctx)
        {
            UI_Cancel?.Invoke(ctx);
        }

        public void OnUI_Point(InputAction.CallbackContext ctx)
        {
            UI_Point?.Invoke(ctx);
        }

        public void OnUI_Click(InputAction.CallbackContext ctx)
        {
            UI_Click?.Invoke(ctx);
        }

        public void OnUI_RightClick(InputAction.CallbackContext ctx)
        {
            UI_RightClick?.Invoke(ctx);
        }

        public void OnUI_MiddleClick(InputAction.CallbackContext ctx)
        {
            UI_MiddleClick?.Invoke(ctx);
        }

        public void OnUI_ScrollWheel(InputAction.CallbackContext ctx)
        {
            UI_ScrollWheel?.Invoke(ctx);
        }

        public void OnUI_TrackedDevicePosition(InputAction.CallbackContext ctx)
        {
            UI_TrackedDevicePosition?.Invoke(ctx);
        }

        public void OnUI_TrackedDeviceOrientation(InputAction.CallbackContext ctx)
        {
            UI_TrackedDeviceOrientation?.Invoke(ctx);
        }

        #endregion
    }
}
