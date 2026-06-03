using Coordinator.Skills;
using Data;
using UnityEngine;

namespace Coordinator.Objects.Weapons
{
    public class HolySword : WeaponCoordinatorBase
    {
        private FistSkill _skill;
        private bool _isPressed = false;
        [SerializeField]
        private int _damage;

        protected override void OnAwake()
        {
            base.OnAwake();
            var go = transform.Find("@AttackBox");
            _skill = go.GetComponent<FistSkill>();

#if UNITY_EDITOR
            Debug.Assert( _skill != null, $"{name} 의 자식중에 @AttackBoxㄸ는 FistSkill이 없음");
#endif
        }
        public override void Init(ObjectData data)
        {
            base.Init(data);
            _isPressed = false;
            _skill.Init(0, _damage);
        }

        public override bool Drop(in Vector2 parentLinVelocity)
        {
            bool ret = base.Drop(parentLinVelocity);
            StopAttack();
            return ret;
        }

        public override bool Throw(in Vector2 dir, in Vector2 parentLinVelocity, float force, int chargeRate)
        {
            bool ret = base.Throw(dir, parentLinVelocity, force, chargeRate);
            StopAttack();
            return ret;
        }

        public override void OnLMBPressed()
        {
            _isPressed = true;
        }

        public override void OnLMBReleased()
        {
            if(_isPressed == false)
            {
                return;
            }
            _skill.SetAttackableLayer(_attackableLayers);
            _skill.Attack(Defines.AttackStatus.PRESSED, 0);
            _weaponUseCnt--;
            if(CanUseWeapon() == false)
            {
                StopAttack();
            }
            _isPressed = false;
        }

        public override void StopAttack()
        {
            _isPressed = false;
        }
    }
}