using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{

    public class PlayerCoordinator : MonoBehaviour
    {
        private EquipmentFactory _itemFactory = new EquipmentFactory();
        private ModeManageCoordinator _modeMgr;
        private PlayerData _data;
        private PlayerVictimCoordinator _victim;
        private Rigidbody2D _rb2d;
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();

            _modeMgr = GetComponentInChildren<ModeManageCoordinator>();

            _data = new PlayerData(Managers.Instance.DataManager.PlayerData);
        }
        public void Init()
        {
            var modes = _modeMgr.GetModeList();
            for (int i = 0; i < modes.Length; i++)
            {
                modes[i].gameObject.SetActive(false);
            }
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
            _victim.Init(_data.MaxHP, _data.MaxHP, _data.InvincibilityTime, _data.HPSoundKeysField);

            var save = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.EquippedItems;
            EquipItem(0, save[0],true);
            EquipItem(1, save[1],true);
            EquipItem(2, save[2],true);
        }


        public void Respawn()
        {
            _victim.Respawn();
            _rb2d.linearVelocity = Vector2.zero;
            _modeMgr.ChangeMode(Defines.ModeTypes.FISTIVAL);
        }

        public void OnModeEnterAnimation()
        {
            _modeMgr.OnEnterAnimationEnd();
        }

        public void OnModeExitAnimation()
        {
            _modeMgr.OnExitAnimationEnd();
        }

        public void OnAnimatorDeadEnd()
        {
            Managers.Instance.StageManager.OnDead();
        }

        public void EquipItem(int slot, int item, bool init=false)
        {
            var save = Managers.Instance.SaveDataManager.GetSaveFileData();

            if(init==false)
            {
                _itemFactory.GetEquipment(save.PlayerSaveData.EquippedItems[slot])?.OnUnEquip(this);
            }
            _itemFactory.GetEquipment(item)?.OnEquip(this);

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

            Debug.Log(_data.MaxLife);
        }
    }
}