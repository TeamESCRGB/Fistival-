using Data;
using UnityEngine;

namespace Manager.Contents
{
    public class StageManager
    {
        private int _stageIdx = -1;
        private StageData _data;

        public void Init()
        {
            _stageIdx = -1;
            _data = null;
        }

        public void ClearPlayDatas()
        {

        }

        public void TrySyncToPlayerData()
        {

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