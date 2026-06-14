using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{

    public class PlayerCoordinator : MonoBehaviour
    {
        private PlayerData _data;
        public void Init()
        {
            _data = new PlayerData(Managers.Instance.DataManager.PlayerData);
            var modeMgr = GetComponentInChildren<ModeManageCoordinator>();
            modeMgr.UnlockMode(Defines.ModeTypes.FISTIVAL);
            modeMgr.ChangeMode(Defines.ModeTypes.FISTIVAL);

            var hitbox = transform.Find("@Hitbox");
            PlayerVictimCoordinator victim = null;
#if UNITY_EDITOR
            Debug.Assert(hitbox != null,"@Hitbox가 없습니다.");
#endif
            if(hitbox != null)
            {
                victim = hitbox.GetComponent<PlayerVictimCoordinator>();
            }
#if UNITY_EDITOR
            Debug.Assert(victim != null, "@Hitbox에 PlayerVictimCoordinator가 없습니다.");
#endif
            victim.Init(_data.MaxHP, _data.MaxHP, _data.InvincibilityTime);

            InitEquipments();
        }

        public PlayerData GetPlayerData()
        {
            return _data;
        }

        public void InitEquipments()
        {
            //여기에 플레이어 상태 초기화 코드 및 효과 적용 작성
        }
    }
}