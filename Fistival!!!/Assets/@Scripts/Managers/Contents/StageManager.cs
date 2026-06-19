using Coordinator.Stages;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class StageManager
    {
        private int _stageIdx = -1;
        private StageData _data;

        private double _scaledTimeStart = 0;
        private double _unscaledTimeStart = 0;
        private int _collectedMoney = 0;

        private Dictionary<string, (StageSectionCoordinatorBase chunk, string allocatedResources)> _spawnedChunks = new Dictionary<string, (StageSectionCoordinatorBase chunk, string allocatedResources)>();

        public void Init()
        {
            _stageIdx = -1;
            _data = null;
            _scaledTimeStart = 0;
            _unscaledTimeStart = 0;
            _collectedMoney = 0;
            
            foreach(var chunk in _spawnedChunks.Values)
            {
                if(chunk.chunk != null)
                {
                    chunk.chunk.DeInitChunk();
                }
                Managers.Instance.ResourceManager.ReleaseIn(chunk.allocatedResources);
            }

            _spawnedChunks.Clear();
        }

        public void TrySyncToPlayerData()
        {

        }


        public void StartStage()
        {
            _scaledTimeStart = _unscaledTimeStart = Time.timeAsDouble;


        }

        public StageSectionCoordinatorBase TrySpawnChunk(string key,string resourceKey ,Vector3 spawnPos)
        {
            var chunk = Managers.Instance.ResourceManager.Instantiate(key);
            if(chunk == null)
            {
                return null;
            }

            chunk.transform.position = spawnPos;

            StageSectionCoordinatorBase section = chunk.GetComponent<StageSectionCoordinatorBase>();

            if(section == null)
            {
                Managers.Instance.ResourceManager.Destroy(chunk);
                return null;
            }

            section.InitChunk();

            _spawnedChunks[key] = (section, resourceKey);

            return section;
        }

        public void TryUnloadChunk(string key)
        {
            if(_spawnedChunks.TryGetValue(key,out var chunk) ==false)
            {
                return;
            }
            _spawnedChunks.Remove(key);
            chunk.chunk.DeInitChunk();
            Managers.Instance.ResourceManager.Destroy(chunk.chunk.gameObject);
            Managers.Instance.ResourceManager.ReleaseIn(chunk.allocatedResources);
        }

        public StageData GetStageData()
        {
            return _data;
        }

        public void SetStageIDX(int idx)
        {
            _stageIdx = idx;
            _data = Managers.Instance.DataManager.StageDataDict[idx];
        }
    }
}