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

            _footCoord.Init(_stepAttackableMask, strongDamage);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _footCoord = GetComponentInChildren<PlatformerFootCoordinator>();
        }
    }
}
