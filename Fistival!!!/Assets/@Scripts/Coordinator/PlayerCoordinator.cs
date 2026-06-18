using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{

    public class PlayerCoordinator : MonoBehaviour
    {
        private ItemFactory _itemFactory = new ItemFactory();
        private ModeManageCoordinator _modeMgr;
        private PlayerData _data;
        private PlayerVictimCoordinator _victim;
        public void Init()
        {
            _data = new PlayerData(Managers.Instance.DataManager.PlayerData);
            _modeMgr = GetComponentInChildren<ModeManageCoordinator>();
            _modeMgr.UnlockMode(Defines.ModeTypes.FISTIVAL);
            _modeMgr.ChangeMode(Defines.ModeTypes.FISTIVAL);

            var hitbox = transform.Find("@Hitbox");
            _victim = null;
#if UNITY_EDITOR
            Debug.Assert(hitbox != null,"@Hitbox가 없습니다.");
#endif
            if(hitbox != null)
            {
                _victim = hitbox.GetComponent<PlayerVictimCoordinator>();
            }
#if UNITY_EDITOR
            Debug.Assert(_victim != null, "@Hitbox에 PlayerVictimCoordinator가 없습니다.");
#endif
            _victim.Init(_data.MaxHP, _data.MaxHP, _data.InvincibilityTime);

            var save = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.EquippedItems;
            EquipItem(0, save[0]);
            EquipItem(1, save[1]);
            EquipItem(2, save[2]);

        }

        public void EquipItem(int slot, int item)
        {
            var save = Managers.Instance.SaveDataManager.GetSaveFileData();

            _itemFactory.GetItem(save.PlayerSaveData.EquippedItems[slot])?.OnUnEquip(this);
            _itemFactory.GetItem(item)?.OnEquip(this);

            save.PlayerSaveData.EquippedItems[slot] = item;
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

            if(_victim != null)
            {
                _victim.SetMaxHP(_data.MaxHP);
            }
        }
    }
}