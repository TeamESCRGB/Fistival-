using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> stageDatas;
        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
            foreach(var data in stageDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}
