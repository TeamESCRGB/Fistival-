using System.Collections.Generic;

namespace Data.DataLoaders
{
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> projectileDatas;
        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach(var data in projectileDatas)
            {
                dict.Add(data.IDX, data);
            }

            return dict;
        }
    }
}
