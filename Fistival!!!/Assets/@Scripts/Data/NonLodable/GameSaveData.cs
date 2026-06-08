namespace Data.NonLodable
{
    public class GameSaveData
    {
        public bool IsEmpty { get; set; } = true;
        public SaveFile[] SaveData { get; set; } = new SaveFile[7];

        public void ClearAllData()
        {
            IsEmpty = true;
            for(int i = 0; i < SaveData.Length; i++)
            {
                SaveData[i]?.ClearAllData();
            }
        }
    }
}
