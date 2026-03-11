using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class CommonModeDataLoader : ILoader<int, CommonModeData>
    {
        public List<CommonModeData> commonModeDatas;
        public Dictionary<int, CommonModeData> MakeDict()
        {
            Dictionary<int, CommonModeData> dict = new Dictionary<int, CommonModeData>();
            foreach(var data in commonModeDatas)
            {
                dict.Add(data.CommonModeDataIdx, data);
            }

            return dict;
        }
    }
}
