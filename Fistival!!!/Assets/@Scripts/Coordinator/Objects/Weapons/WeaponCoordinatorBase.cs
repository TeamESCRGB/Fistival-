using Data;
using UnityEngine;

namespace Coordinator.Objects.Weapons
{
    public abstract class WeaponCoordinatorBase : ObjectCoordinator
    {
        [SerializeField]
        protected int _maxWeaponUseCnt = 3;
        protected int _weaponUseCnt = 3;
        [SerializeField]
        protected string _animName;

        [SerializeField]
        private bool _hasInternalTimer=false;

        public override void Init(ObjectData data)
        {
            base.Init(data);
            _weaponUseCnt = _maxWeaponUseCnt;
        }

        public abstract void StopAttack();

        public bool CanUseWeapon()
        {
            return _weaponUseCnt > 0;
        }

        public string GetAnimKey()
        {
            return _animName;
        }


        //cooldown들은 자체적인 쿨타임 체계를 따르는 무기와의 로직을 공유해서 코드가 난잡해지는걸 막기 위해 넣었습니다.
        public virtual bool IsCooldownEnd()
        {
            return true;
        }

        public virtual void StartCooldown()
        {
            
        }

        public bool HasInternalTimer()
        {
            return _hasInternalTimer;
        }

        public abstract void OnLMBReleased();

        public abstract void OnLMBPressed();
    }
}