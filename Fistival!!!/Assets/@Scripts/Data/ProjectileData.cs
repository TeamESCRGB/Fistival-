using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class ProjectileData
    {
        public ProjectileData(ProjectileData original)
        {
            IDX = original.IDX;
        }
        public ProjectileData() { }

        public int IDX;
    }
}
