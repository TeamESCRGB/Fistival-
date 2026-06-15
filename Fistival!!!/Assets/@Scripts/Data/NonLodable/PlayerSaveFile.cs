using System.Collections.Generic;

namespace Data.NonLodable
{
    public class PlayerSaveFile
    {
        public int Money { get; set; } = 0;
        public List<int> PurchasedItems { get; set; } = new List<int>();
        public int[] EquippedItems { get; set; } = new int[3] { -1,-1,-1};
        public List<int> UnlockedTrainingStage { get; set; } = new List<int>();

        public void ClearAllData()
        {
            Money = 0;
            PurchasedItems.Clear();
            EquippedItems[0] = -1;
            EquippedItems[1] = -1;
            EquippedItems[2] = -1;
            UnlockedTrainingStage.Clear();
        }
    }
}