using Coordinator.Door;
using Coordinator.Mobs;
using Data;
using Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.Trigger.PlatformerStage
{
    public class CameleonSpawnTrigger : MobSpawnTriggerBase<int>
    {
        [SerializeField]
        private Transform _center;
        [SerializeField]

        private Transform _objSpawnPoint;
        [SerializeField]
        //지점들은 좌->우 방향입니다. 왼쪽 끝이 왼쪽 벽에 붙는 지점, 오른쪽 끝이 오른쪽 벽에 붙는 지점입니다.
        //첫번째 원소가 왼쪽 끝, 마지막 원소가 오른쪽 끝입니다.
        private List<Transform> _movablePoints;
        private CommonMobData _data;
        private bool _isSpawned;
        [SerializeField]
        private GameObject[] _doorObj;
        private List<IDoor> _door;

        private void Awake()
        {
            for (int i = 0; i < _doorObj.Length; i++)
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

        public override void Init()
        {
            _isSpawned = false;
        }

        protected override void OnTrigger(GameObject triggeredObject)
        {
            if (_isSpawned || Managers.Instance.StageManager.IsBossCleared(_data.PrefabKey))
            {
                return;
            }

            var go = Managers.Instance.ResourceManager.Instantiate(_data.PrefabKey, null, true, true);
            if (go == null)
            {
                return;
            }

            if (go.TryGetComponent<CameleonBossMob>(out var comp) == false)
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return;
            }
            go.transform.position = _mobSpawnPoint.position;
            comp.Init(_data,_center.position, _movablePoints, _objSpawnPoint, _door);
            _isSpawned = true;
        }
    }
}