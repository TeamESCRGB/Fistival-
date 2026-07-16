using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class TalkScriptDataLoader : ILoader<string,TalkScriptData>
    {
        public List<TalkScriptData> talkScriptDatas;
        public Dictionary<string, TalkScriptData> MakeDict()
        {
            Dictionary<string, TalkScriptData> dict = new Dictionary<string, TalkScriptData>();
            foreach (var data in talkScriptDatas)
            {
                dict.Add(data.DataKey, data);
            }

            return dict;
        }
    }
}
