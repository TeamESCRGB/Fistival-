using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class CommonMobDataLoader : ILoader<int, CommonMobData>
    {
        public List<CommonMobData> commonMobDatas;
        public Dictionary<int, CommonMobData> MakeDict()
        {
            Dictionary<int, CommonMobData> dict = new Dictionary<int, CommonMobData>();
            foreach(var data in commonMobDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}
