using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{

    public class PlayerCoordinator : MonoBehaviour
    {
        private ModeManageCoordinator _modeMgr;
        private PlayerData _data;
        public void Init()
        {
            _data = new PlayerData(Managers.Instance.DataManager.PlayerData);
            _modeMgr = GetComponentInChildren<ModeManageCoordinator>();
            _modeMgr.UnlockMode(Defines.ModeTypes.FISTIVAL);
            _modeMgr.ChangeMode(Defines.ModeTypes.FISTIVAL);

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

        }

        public PlayerData GetPlayerData()
        {
            return _data;
        }

        public void UpdateUpdatedDatas()
        {
            ModeBase mode = null;
            if(_modeMgr != null)
            {
                mode = _modeMgr.GetNowMode();
            }

            if(mode != null)
            {
                mode.UpdateUpdatedPlayerData();
            }
        }
    }
}