using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class EquipmentDataLoader : ILoader<int,EquipmentData>
    {
        public List<EquipmentData> equipmentDatas;
        public Dictionary<int, EquipmentData> MakeDict()
        {
            Dictionary<int, EquipmentData> dict = new Dictionary<int, EquipmentData>();
            foreach (var data in equipmentDatas)
            {
                dict.Add(data.Idx, data);
            }

            return dict;
        }
    }
}
