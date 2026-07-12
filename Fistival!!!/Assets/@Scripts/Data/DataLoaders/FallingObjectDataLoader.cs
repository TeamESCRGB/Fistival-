using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class FallingObjectDataLoader : ILoader<int, FallingObjectData>
    {
        public List<FallingObjectData> fallingObjectDatas;
        public Dictionary<int, FallingObjectData> MakeDict()
        {
            Dictionary<int, FallingObjectData> dict = new Dictionary<int, FallingObjectData>();
            foreach(var data in fallingObjectDatas)
            {
                dict.Add(data.IDX, data);
            }

            return dict;
        }
    }
}
