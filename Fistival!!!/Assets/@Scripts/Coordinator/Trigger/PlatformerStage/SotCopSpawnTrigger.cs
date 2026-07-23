using Coordinator.Mobs;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator.Trigger.PlatformerStage
{
    public class SotCopSpawnTrigger : MobSpawnTriggerBase<int>
    {
        private bool _isSpawned;
        private CommonMobData _data;
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
            comp.Init(_data);
            _isSpawned = true;
        }

        public override void Init()
        {
            _isSpawned = false;
        }
    }
}