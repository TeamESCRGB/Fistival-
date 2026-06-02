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

        public abstract void OnLMBReleased();

        public abstract void OnLMBPressed();
    }
}