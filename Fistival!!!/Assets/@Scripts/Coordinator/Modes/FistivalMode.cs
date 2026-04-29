using Data;
using UnityEngine;
using Utils;
using Coordinator.Hands;
using Coordinator.Movements;
using Defines;

namespace Coordinator.Modes
{
    public class FistivalMode : ModeBase
    {
        protected HandCoordinator _hand;
        private PlatformerMovementCoordinator _movCoordinator;
        private float _objectWeight = 0;
        public override ModeTypes ModeType => ModeTypes.FISTIVAL;


        protected override void OnAwake()
        {
            base.OnAwake();
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
            _hand = GetComponentInChildren<HandCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(data.MoveSpeed,data.JumpPower,_commonData.SlownessSensitivity,_commonData.MaxSlowness,GetComponentInParent<Rigidbody2D>());
            _inputCoordinator.SetJumpsMovementInputHandler(_movCoordinator);
            _inputCoordinator.SetHorizontalMovementInputHandler(_movCoordinator);
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

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed)
            {
                _hand.OnRMBPressed();
            }
            else
            {
                _hand.SetMousePos(screenPos);
                _hand.OnRMBReleased();
            }
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed)
            {
                _hand.OnLMBPressed();
            }
            else
            {
                _hand.OnLMBReleased();
            }
        }

        public override void OnDropEvent(bool pressed)
        {
            _hand.Drop();
        }
    }
}