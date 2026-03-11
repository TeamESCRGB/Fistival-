using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class ObjectDataLoader : ILoader<int, ObjectData>
    {
        public List<ObjectData> objectDatas;
        public Dictionary<int, ObjectData> MakeDict()
        {
            Dictionary<int, ObjectData> dict = new Dictionary<int, ObjectData>();
            foreach(var data in objectDatas)
            {
                dict.Add(data.ObjectIndex, data);
            }

            return dict;
        }
    }
}
