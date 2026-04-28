using Data;
using Defines;
using Coordinator.Skills;

namespace Coordinator.Modes
{
    public class PlatformerMode : FistivalMode
    {
        public override ModeTypes ModeType => ModeTypes.PLATFORMER;
        private PlatformerFootCoordinator _footCoord;

        public override void Init(CommonModeData data)
        {
            base.Init(data);

            int strongDamage = data.Damage * _hand.GetStrongAttackDamageMultiplier();//여기에 공격력 증가치도 나중에 인자 받아서 넣어두기 TODO

            _footCoord.Init(data.AttackableLayers, strongDamage);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _footCoord = GetComponentInChildren<PlatformerFootCoordinator>();
        }
    }
}
