using System.Collections.Generic;

namespace Data.NonLodable
{
    public class StageSaveFile
    {
        public bool IsCleared { get; set; } = false;
        public double ClearTimeWithOutPause { get; set; } = -1;
        public double ClearTimeWithPause { get; set; } = -1;
        public int TotalGainedDamage { get; set; } = 0;
        public List<int> CollectedCollections { get; set; }= new List<int>(5);

        public void ClearAllData()
        {
            IsCleared = false;
            ClearTimeWithOutPause = -1;
            ClearTimeWithPause = -1;
            TotalGainedDamage = 0;
            CollectedCollections.Clear();
        }
    }
}