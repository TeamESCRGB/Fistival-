using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
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

        public override ModeTypes ModeType => ModeTypes.RHYTHM;

        private void Awake()
        {
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
            _movementCoordinator.SetSlowness(1 / (1 + (now / max * _objectWeight)));
        }

        public void OnDropEvent(bool pressed)
        {
            if(pressed)
            {
                _hand.Drop();
            }
        }

        public void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            _hand.SetMousePos(screenPos);
            if(pressed)
            {
                _hand.OnLMBPressed();
            }
            else
            {
                _hand.OnLMBReleased();
            }
        }

        public void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if (pressed)
            {
                _hand.OnRMBPressed();
            }
            else
            {
                _hand.SetMousePos(screenPos);
                _hand.OnRMBReleased();
            }
        }
    }
}