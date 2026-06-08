namespace Data.NonLodable
{
    public class GameSaveData
    {
        public SaveFile[] SaveData { get; set; } = new SaveFile[7];

        public void ClearAllData()
        {
            for(int i = 0; i < SaveData.Length; i++)
            {
                SaveData[i]?.ClearAllData();
            }
        }
    }
}
