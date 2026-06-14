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
            //체력 초기화 코드 작성

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