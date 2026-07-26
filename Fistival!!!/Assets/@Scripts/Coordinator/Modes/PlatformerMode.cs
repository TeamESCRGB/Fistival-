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

            _footCoord.gameObject.SetActive(true);
            _rb2d.transform.GetComponentInChildren<HPCoordinator>().SubscribeOnDead(OnDead);
        }

        public override void DeInit()
        {
            _rb2d.transform.GetComponentInChildren<HPCoordinator>().UnSubscribeOnDead(OnDead);
            base.DeInit();
        }

        /// <summary>
        /// 이거는 리스폰하면 모드가 무조건 기본모드로 돌아간다는 전제 하에 작동합니다. 만약 나중에 기획 바뀌면 이거 고쳐야 합니다.
        /// 리스폰 콜백 만들고, 리스폰하면 다시 켜주는 그런방식으로
        /// </summary>
        protected void OnDead()
        {
            _footCoord.gameObject.SetActive(false);
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
