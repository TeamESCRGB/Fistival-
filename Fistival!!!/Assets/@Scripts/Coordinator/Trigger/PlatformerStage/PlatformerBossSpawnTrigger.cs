using Coordinator.Mobs;
using Data;
using Manager;
using System.Collections.Generic;
using UI.Popup;
using UnityEngine;

namespace Coordinator.Trigger
{
    public class PlatformerBossSpawnTrigger : MobSpawnTriggerBase<int>
    {
        private Transform _objSpawnPosMin, _objSpawnPosMax, _phase2MobSpawnPos;
        private BlockWaveCoordinator _phase2BlockWaveCoordinator;
        private GameObject _phase2PlatformObj;
        private List<Transform> _phase2ObjSpawnPoints;
        private bool _isSpawned;
        private bool _isTalking;
        private CommonMobData _data;
        protected override void OnStart()
        {
            base.OnStart();
            _isTalking = false;
            _isSpawned = false;
            Managers.Instance.DataManager.CommonMobDataDict.TryGetValue(_mobDataIdx, out _data);
        }

        public override void Init()
        {
            _isTalking = false;
            _isSpawned = false;
        }

        protected override void OnTrigger(GameObject triggeredObject)
        {
            if (_isTalking || _isSpawned || Managers.Instance.StageManager.IsBossCleared(_data.PrefabKey))
            {
                return;
            }

            var talkUI = Managers.Instance.UIManager.ShowPopupUI<TalkCutScenePopup>("TalkCutScenePopup").SetData("PlatformerBoss1StartCutScene").SetOnEnd(() => {
                _isTalking = false;
                Spawn();
            });

        }

        private void Spawn()
        {
            var go = Managers.Instance.ResourceManager.Instantiate(_data.PrefabKey, null, true, true);
            if (go == null)
            {
                return;
            }

            if (go.TryGetComponent<PlatformerBossPhase1Mob>(out var comp) == false)
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return;
            }
            go.transform.position = _mobSpawnPoint.position;
            comp.Init(_data,
                _objSpawnPosMin.position, _objSpawnPosMax.position,
                _phase2MobSpawnPos,
                _phase2ObjSpawnPoints,
                _phase2BlockWaveCoordinator,
                _phase2PlatformObj
                );
            _isSpawned = true;
        }
    }
}