using System.Collections.Generic;

namespace Data.NonLodable
{
    public class SaveFile
    {
        public double TotalPlayTime { get; set; } = 0;
        public PlayerSaveFile PlayerSaveData { get; set; } = new PlayerSaveFile();
        public Dictionary<int, StageSaveFile> StageSaveDatas { get; set; } = new Dictionary<int, StageSaveFile>();
    }
}