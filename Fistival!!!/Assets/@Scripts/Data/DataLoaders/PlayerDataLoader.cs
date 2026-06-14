using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class PlayerDataLoader : ILoader<int,PlayerData>
    {
        public PlayerData playerData;
        public Dictionary<int, PlayerData> MakeDict()
        {
            Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();

            dict.Add(0, playerData);

            return dict;
        }

        public PlayerData GetData()
        {
            return playerData;
        }
    }
}
