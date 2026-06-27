using Data;
using Defines;
using Coordinator.Skills;
using UnityEngine;

namespace Coordinator.Modes
{
    public class PlatformerMode : FistivalMode
    {
        [SerializeField]
        private LayerMask _stepAttackableMask;
        public override ModeTypes ModeType => ModeTypes.PLATFORMER;
        private PlatformerFootCoordinator _footCoord;

        public override void Init(CommonModeData data)
        {
            base.Init(data);

            int strongDamage = _playerData.Damage + _playerData.StrongAttackDamage;

            _footCoord.Init(_stepAttackableMask, strongDamage, _playerData.StunTime + _playerData.StrongStunTime);
        }

        public override void UpdateUpdatedPlayerData()
        {
            base.UpdateUpdatedPlayerData();
            _footCoord.SetBaseDamage(_playerData.Damage + _playerData.StrongAttackDamage);
            //_footCoord업데이트 해줘야함 함
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _footCoord = GetComponentInChildren<PlatformerFootCoordinator>();
        }
    }
}
