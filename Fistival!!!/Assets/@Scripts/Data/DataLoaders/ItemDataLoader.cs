using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class ItemDataLoader : ILoader<int,ItemData>
    {
        public List<ItemData> itemDatas;
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
            foreach (var data in itemDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}
