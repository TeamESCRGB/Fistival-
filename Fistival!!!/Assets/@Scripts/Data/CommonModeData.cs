using UnityEngine;

namespace Data
{
    public class CommonModeData
    {
        public CommonModeData(CommonModeData original)
        {
            CommonModeDataIdx = original.CommonModeDataIdx;
            AnimControllerName = original.AnimControllerName;
        }
        public CommonModeData() { }

        public int CommonModeDataIdx;
        public string AnimControllerName;
    }
}