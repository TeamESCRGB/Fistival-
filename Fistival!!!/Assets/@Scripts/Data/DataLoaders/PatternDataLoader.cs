using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataLoaders
{
    public class PatternDataLoader : ILoader<string,PatternData>
    {
        public List<PatternData> patternDatas;
        public Dictionary<string, PatternData> MakeDict()
        {
            Dictionary<string, PatternData> dict = new Dictionary<string, PatternData>();
            foreach (var data in patternDatas)
            {
                dict.Add(data.Key, data);
            }

            return dict;
        }
    }
}
