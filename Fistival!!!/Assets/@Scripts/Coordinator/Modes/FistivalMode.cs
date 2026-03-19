using Data;
using InputHandler;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class FistivalMode : ModeBase, IRMBInputHandler, ILMBInputHandler, IDropInputHandler
    {
        private HandCoordinator _hand;
        private PlatformerMovementCoordinator _movCoordinator;
        private float _objectWeight = 0;
        private void Awake()
        {
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
            ModeType = Defines.ModeTypes.FISTIVAL;
            _hand = GetComponentInChildren<HandCoordinator>();
            _inputCoordinator = gameObject.GetComponentInParent<PlayerInputCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(data.MoveSpeed,data.JumpPower,_commonData.SlownessSensitivity,_commonData.MaxSlowness,GetComponentInParent<Rigidbody2D>());
            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetMovementInputHandler(_movCoordinator);
            _hand.Init(GetComponentInParent<Rigidbody2D>(), data.Damage, data.AttackableLayers);//아니 이거 데이터에 추가해야되네
            _hand.OnGrabbedObjectChanged+=OnGrabbedObjectChanged;
            _hand.OnChargeRateChanged += OnChargeRateChanged;
            _objectWeight = 0;
        }

        public override void DeInit()
        {
            _hand.Drop();
            base.DeInit();
        }

        private void OnGrabbedObjectChanged(ObjectData objData)
        {
            if(objData == null)
            {
                _objectWeight = 0;
            }
            else
            {
                _objectWeight = objData.Weight;
            }

        }

        private void OnChargeRateChanged(int now, int max)
        {
            _movCoordinator.SetSlowness(1/(1 + (now / max * _objectWeight)));
        }

        public void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed)
            {
                _hand.OnRMBPressed();
            }
            else
            {
                _hand.MousePos = screenPos;
                _hand.OnRMBReleased();
            }
        }

        public void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed == false)
            {
                return;
            }
            _hand.Attack();
        }

        public void OnDropEvent(bool pressed)
        {
            _hand.Drop();
        }
    }
}