using InputHandler;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Coordinator
{
    public class PlayerInputCoordinator : MonoBehaviour
    {
        private bool _isDownJumpTriggered = false;
        private ILMBInputHandler _lmbHandler;
        private IRMBInputHandler _rmbHandler;
        private IPointerMovementInputHandler _pointerHandler;
        private IDropInputHandler _dropHandler;
        private IReloadInputHandler _reloadHandler;

        private IJumpsMovementInputHandler _jumpsHandler;
        private IHorizontalMovementInputHandler _horizontalMovementHandler;
        private IVerticalMovementInputHandler _verticalMovementHandler;

        private IInteractionInputHandler _interactionHandler;
        private IESCInputHandler _escHandler;

        private Vector2 _lastPos;

        private void Awake()
        {
            Managers.Instance.NewInputSystemManager.Player_LMBInput += OnLMBEvent;
            Managers.Instance.NewInputSystemManager.Player_RMBInput += OnRMBEvent;
            Managers.Instance.NewInputSystemManager.Player_PointerMovementInput += OnPointerMove;


            Managers.Instance.NewInputSystemManager.Player_LeftMovementInput += OnLeftMovementInputEvent;
            Managers.Instance.NewInputSystemManager.Player_RightMovementInput += OnRightMovementInputEvent;
            Managers.Instance.NewInputSystemManager.Player_UpMovementInput += OnUpMovementInputEvent;
            Managers.Instance.NewInputSystemManager.Player_DownTrigger_for_bug_fix += OnDownMovementInputEvent;
            Managers.Instance.NewInputSystemManager.Player_JumpInput += OnJumpMovementInputEvent;


            Managers.Instance.NewInputSystemManager.Player_ReloadInput += OnReloadInputEvent;
            Managers.Instance.NewInputSystemManager.Player_DropInput += OnDropInput;

            Managers.Instance.NewInputSystemManager.Player_InteractionInput += OnInteractionInputEvent;
            Managers.Instance.NewInputSystemManager.Player_ESCInput += OnESCInputEvent;
        }

        public void SetLMBInputHandler(ILMBInputHandler handler)
        {
            _lmbHandler=handler;
        }
        public void SetRMBInputHandler(IRMBInputHandler handler)
        {
            _rmbHandler=handler;
        }
        public void SetPointerMovementInputHandler(IPointerMovementInputHandler handler)
        {
            _pointerHandler=handler;
        }

        public void SetDropInputHandler(IDropInputHandler handler)
        {
            _dropHandler = handler;
        }


        public void SetReloadInputHandler(IReloadInputHandler handler)
        {
            _reloadHandler = handler;
        }


        public void SetInteractionInputHandler(IInteractionInputHandler handler)
        {
            _interactionHandler = handler;
        }

        public void SetESCInputHandler(IESCInputHandler handler)
        {
            _escHandler = handler;
        }

        #region Movement
        public void SetJumpsMovementInputHandler(IJumpsMovementInputHandler handler)
        {
            _jumpsHandler=handler;
        }

        public void SetVerticalMovementInputHandler(IVerticalMovementInputHandler handler)
        {
            _verticalMovementHandler = handler;
        }

        public void SetHorizontalMovementInputHandler(IHorizontalMovementInputHandler handler)
        {
            _horizontalMovementHandler=handler;
        }
        #endregion



        public void Init()
        {
            _jumpsHandler = null;
            _verticalMovementHandler =null;
            _horizontalMovementHandler = null;

            _reloadHandler = null;
            _isDownJumpTriggered = false;
            _lmbHandler = null;
            _rmbHandler = null;
            _pointerHandler = null;
            _dropHandler = null;

            _interactionHandler = null;
            _escHandler = null;

            _lastPos = Vector2.zero;
        }

        public void TriggerReleaseMovementInput()
        {
            _isDownJumpTriggered = false;

            _verticalMovementHandler?.OnUpMovementInputEvent(false);
            _verticalMovementHandler?.OnDownMovementInputEvent(false);
            _horizontalMovementHandler?.OnLeftMovementInputEvent(false);
            _horizontalMovementHandler?.OnRightMovementInputEvent(false);
        }

        public void OnInteractionInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.performed == false)
            {
                return;//test
            }
            _interactionHandler?.OnInteractionInputEvent(true);//이거ㅓㅓㅓ는 inputsystem솟버그때문에, 일단 상호작용키가 누르는 상황만 단발성으로 감지된다는 전제 하에 해둔겁니다. 만약 그게 아니게되는 상황이 온다면 코드를 고쳐야 합니다.
        }

        public void OnESCInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }

            _escHandler?.OnESCInputEvent(callbackContext.control.IsPressed());
        }

        public void OnReloadInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _reloadHandler?.OnReloadInputEvent(callbackContext.control.IsPressed());
        }

        public void OnUpMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }
            _verticalMovementHandler?.OnUpMovementInputEvent(callbackContext.control.IsPressed());
        }

        public void OnDownMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            if (callbackContext.control.IsPressed())
            {
                _verticalMovementHandler?.OnDownMovementInputEvent(true);
                _isDownJumpTriggered = true;
            }
            else
            {
                _verticalMovementHandler?.OnDownMovementInputEvent(false);
                _isDownJumpTriggered = false;
            }
        }

        public void OnJumpMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }

            if(_isDownJumpTriggered)
            {
                _jumpsHandler?.OnDownJumpMovementInputEvent(callbackContext.control.IsPressed());
            }
            else
            {
                _jumpsHandler?.OnJumpMovementInputEvent(callbackContext.control.IsPressed());
            }
        }

        public void OnLeftMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _horizontalMovementHandler?.OnLeftMovementInputEvent(callbackContext.control.IsPressed());
        }
        public void OnRightMovementInputEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _horizontalMovementHandler?.OnRightMovementInputEvent(callbackContext.control.IsPressed());
        }

        public void OnPointerMove(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _lastPos = callbackContext.ReadValue<Vector2>();
            _pointerHandler?.OnPointerMove(_lastPos);
        }
        public void OnLMBEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _lmbHandler?.OnLMBEvent(callbackContext.control.IsPressed(), _lastPos);
        }
        public void OnRMBEvent(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                return;
            }
            _rmbHandler?.OnRMBEvent(callbackContext.control.IsPressed(), _lastPos);
        }

        public void OnDropInput(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                return;
            }
            _dropHandler?.OnDropEvent(callbackContext.control.IsPressed());
        }
    }
}
