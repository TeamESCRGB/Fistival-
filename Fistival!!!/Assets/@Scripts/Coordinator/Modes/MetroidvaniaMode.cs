using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class MetroidvaniaMode : ModeBase
    {
        private MetroidvaniaHand _hand;
        private PlatformerMovementCoordinator _movCoordinator;
        private float _objectWeight = 0;
        public override ModeTypes ModeType => ModeTypes.METROIDVANIA;

        protected override void OnAwake()
        {
            base.OnAwake();
            _hand = GetComponentInChildren<MetroidvaniaHand>();
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(data.MoveSpeed, data.JumpPower, _commonData.SlownessSensitivity, _commonData.MaxSlowness, GetComponentInParent<Rigidbody2D>());
            _inputCoordinator.SetJumpsMovementInputHandler(_movCoordinator);
            _inputCoordinator.SetHorizontalMovementInputHandler(_movCoordinator);
        }

        public override void DeInit()
        {

            base.DeInit();
        }

        private void OnGrabbedObjectChanged(ObjectData objData)
        {
            if (objData == null)
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
            _movCoordinator.SetSlowness(1 / (1 + (now / max * _objectWeight)));
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new System.NotImplementedException();
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDropEvent(bool pressed)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStunEnd()
        {
            throw new System.NotImplementedException();
        }

        public override void StunFor(float time)
        {
            throw new System.NotImplementedException();
        }

        public override void ReleaseStun()
        {
            throw new System.NotImplementedException();
        }
    }
}