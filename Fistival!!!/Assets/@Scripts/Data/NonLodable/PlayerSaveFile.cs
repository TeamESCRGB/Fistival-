using System.Collections.Generic;

namespace Data.NonLodable
{
    public class PlayerSaveFile
    {
        public int Money { get; set; } = 0;
        public List<int> PurchasedItems { get; set; } = new List<int>();
        public int[] EquippedItems { get; set; } = new int[3];
        public List<int> UnlockedTrainingStage { get; set; } = new List<int>();
    }
}