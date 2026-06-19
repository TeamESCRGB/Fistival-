using Data;
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