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
    public class WWEMode : ModeBase, IVerticalMovementInputHandler, IHorizontalMovementInputHandler
    {
        private WWEHandCoordinator _hand;
        private PlatformerMovementCoordinator _movCoordinator;
        private float _objectWeight = 0;

        public override ModeTypes ModeType => ModeTypes.WWE;

        protected override void OnAwake()
        {
            base.OnAwake();
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
            _hand = GetComponentInChildren<WWEHandCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(data.MoveSpeed, data.JumpPower, data.SlownessSensitivity, data.MaxSlowness, GetComponentInParent<Rigidbody2D>());
            _hand.Init(GetComponentInParent<Rigidbody2D>(), data.Damage, data.AttackableLayers, data.PickableLayers, data.ForcePerCharge, data.ChargeTimeInterval, data.AttackCooldown);

            _hand.OnGrabbedObjectChanged += OnGrabbedObjectChanged;
            _hand.OnChargeRateChanged += OnChargeRateChanged;
            _inputCoordinator.SetJumpsMovementInputHandler(_movCoordinator);//좌우이동은 여기에서 처리해야 할 추가적인 일이 있어서 대기
            _inputCoordinator.SetHorizontalMovementInputHandler(this);
            _inputCoordinator.SetVerticalMovementInputHandler(this);
            _objectWeight = 0;
        }

        public override void DeInit()
        {
            _hand.Drop();
            _hand.StopAttack();
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
        public void OnDownMovementInputEvent(bool pressed)
        {
            if (pressed && (_isStunned == false))
            {
                _hand.SetComboType(WWESkillTypes.SYOURYUUKEN);
            }
        }

        public void OnUpMovementInputEvent(bool pressed)
        {
            if (pressed && (_isStunned == false))
            {
                _hand.SetComboType(WWESkillTypes.TATSUMAKISENPUKYAKU);
            }
        }

        public void OnLeftMovementInputEvent(bool pressed)
        {
            _movCoordinator.OnLeftMovementInputEvent(pressed);
            if (_isStunned)
            {
                return;
            }
            if (pressed)
            {
                _hand.SetComboType(WWESkillTypes.HADOUKEN);
            }
        }

        public void OnRightMovementInputEvent(bool pressed)
        {
            _movCoordinator.OnRightMovementInputEvent(pressed);
            if (_isStunned)
            {
                return;
            }
            if (pressed)
            {
                _hand.SetComboType(WWESkillTypes.HADOUKEN);
            }
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(_isStunned)
            {
                return;
            }
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
            if (_isStunned)
            {
                return;
            }
            if (pressed)
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
            if (pressed && (_isStunned == false))
            {
                _hand.Drop();
            }
        }
        protected override void OnStunEnd()
        {
            _isStunned = false;
            _movCoordinator.UnlockMovement();
        }

        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _movCoordinator.LockMovement();
                _hand.Drop();
                _hand.StopAttack();
            }

            _stunCounter.SetCooldownTime(time);
            _stunCounter.StartCooldown();
            _isStunned = true;
        }

        public override void ReleaseStun()
        {
            if (_stunCounter is null || _stunCounter.IsCooldownEnded())
            {
                return;
            }
            _stunCounter.StopCooldown();
        }
    }
}