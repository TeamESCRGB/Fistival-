using System.Collections.Generic;

namespace Data.NonLodable
{
    public class SaveFile
    {
        public bool IsEmpty { get; set; } = true;
        public double TotalPlayTime { get; set; } = 0;
        public PlayerSaveFile PlayerSaveData { get; set; } = new PlayerSaveFile();
        public Dictionary<int, StageSaveFile> StageSaveDatas { get; set; } = new Dictionary<int, StageSaveFile>();

        public void ClearAllData()
        {
            IsEmpty = true;
            TotalPlayTime = 0;
            PlayerSaveData.ClearAllData();
            foreach(var stage in StageSaveDatas.Values)
            {
                stage.ClearAllData();
            }
        }
    }
}