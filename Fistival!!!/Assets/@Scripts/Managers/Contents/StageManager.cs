using Coordinator.Stages;
using Data;
using System;
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

        public void Init()
        {
            _stageIdx = -1;
            _data = null;
            _scaledTimeStart = 0;
            _unscaledTimeStart = 0;
            _collectedMoney = 0;
        }

        public void TrySyncToPlayerData()
        {

        }


        public void StartStage()
        {
            _scaledTimeStart = _unscaledTimeStart = Time.timeAsDouble;


        }

        public StageSectionCoordinatorBase TrySpawnChunk(string key, Vector3 spawnPos)
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

            return section;
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