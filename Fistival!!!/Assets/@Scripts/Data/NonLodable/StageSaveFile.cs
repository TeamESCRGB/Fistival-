using System.Collections.Generic;

namespace Data.NonLodable
{
    public class StageSaveFile
    {
        public bool IsCleared { get; set; } = false;
        public double ClearTimeWithOutPause { get; set; } = 0;
        public double ClearTimeWithPause { get; set; } = 0;
        public int TotalGainedDamage { get; set; } = 0;
        public List<int> CollectedCollections { get; set; }= new List<int>(4);
    }
}