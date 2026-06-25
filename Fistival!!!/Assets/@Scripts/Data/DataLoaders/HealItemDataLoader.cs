using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataLoaders
{
    public class HealItemDataLoader : ILoader<int, HealItemData>
    {
        public List<HealItemData> healItemDatas;
        public Dictionary<int,HealItemData> MakeDict()
        {
            Dictionary<int, HealItemData> dict = new Dictionary<int, HealItemData>();
            foreach (var data in healItemDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}