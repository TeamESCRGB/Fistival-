using Coordinator.Door;
using Coordinator.Mobs;
using Data;
using Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.Trigger.PlatformerStage
{
    public class SotCopSpawnTrigger : MobSpawnTriggerBase<int>
    {
        private bool _isSpawned;
        private CommonMobData _data;
        [SerializeField]
        private GameObject[] _doorObj;
        private List<IDoor> _door = new List<IDoor>();
        [SerializeField]
        private int _spawnCnt = 0;//만약 두번씩 스폰되는 버그가 또 나오면, 이 변수를 확인하고 디버깅을 하면 될겁니다... 저는 도저히 원인 못찾겠음

        private void Awake()
        {
            for(int i = 0; i < _doorObj.Length; i++)
            {
                _door.Add(_doorObj[i].GetComponent<IDoor>());
#if UNITY_EDITOR
                Debug.Assert(_door[i] != null, $"{_doorObj[i].name} 에 IDoor를 받은 클래스가 없습니다");
#endif
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            _isSpawned = false;
            Managers.Instance.DataManager.CommonMobDataDict.TryGetValue(_mobDataIdx, out _data);
        }
        protected override void OnTrigger(GameObject triggeredObject)
        {
            if(_isSpawned || Managers.Instance.StageManager.IsBossCleared(_data.PrefabKey))
            {
                return;
            }

            Debug.Log("sot spawn");

            var go = Managers.Instance.ResourceManager.Instantiate(_data.PrefabKey,null,true,true);
            
            if(go == null)
            {
                return;
            }

            if(go.TryGetComponent<SotCopMob>(out var comp) == false)
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return;
            }
            go.transform.position = _mobSpawnPoint.position;
            comp.Init(_data, _door);
            _isSpawned = true;
            _spawnCnt++;
        }

        public override void Init()
        {
            _isSpawned = false;
            _spawnCnt = 0;
            Debug.Log("init of sotcopspawner");
        }
    }
}