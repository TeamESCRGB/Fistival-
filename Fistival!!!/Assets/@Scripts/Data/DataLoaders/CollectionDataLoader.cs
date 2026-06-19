using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class CollectionDataLoader : ILoader<int, CollectionData>
    {
        public List<CollectionData> collectionDatas;
        public Dictionary<int, CollectionData> MakeDict()
        {
            Dictionary<int, CollectionData> dict = new Dictionary<int, CollectionData>();
            foreach(var data in collectionDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}
