using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using InputHandler;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class RhythmMode : ModeBase, IRMBInputHandler, ILMBInputHandler, IDropInputHandler
    {

        private RhythmHandCoordinator _hand;
        private PlatformerMovementCoordinator _movementCoordinator;
        private float _objectWeight = 0;

        private void Awake()
        {
            ModeType = Defines.ModeTypes.RHYTHM;
            _hand = gameObject.GetComponentInChildren<RhythmHandCoordinator>();
            _inputCoordinator = gameObject.GetComponentInParent<PlayerInputCoordinator>();
            _movementCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            //초기화 로직
            _hand.Init(GetComponentInParent<Rigidbody2D>(), data.Damage, data.AttackableLayers);
            _movementCoordinator.Init(data.MoveSpeed, data.JumpPower, _commonData.SlownessSensitivity, _commonData.MaxSlowness, GetComponentInParent<Rigidbody2D>());

            _objectWeight = 0;

            _inputCoordinator.SetDropInputHandler(this);
            _inputCoordinator.SetLMBInputHandler(this);
            _inputCoordinator.SetRMBInputHandler(this);
            _inputCoordinator.SetMovementInputHandler(_movementCoordinator);

            _hand.OnGrabbedObjectChanged += OnGrabbedObjectChanged;
            _hand.OnChargeRateChanged += OnChargeRateChanged;
        }

        public override void DeInit()
        {
            //해제 로직
            _hand.Drop();
            base.DeInit();
        }


        private void OnGrabbedObjectChanged(ObjectData objData)
        {
            throw new NotImplementedException();
        }

        private void OnChargeRateChanged(int now, int max)
        {
            throw new NotImplementedException();
        }

        public void OnDropEvent(bool pressed)
        {
            throw new NotImplementedException();
        }

        public void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new NotImplementedException();
        }

        public void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new NotImplementedException();
        }
    }
}