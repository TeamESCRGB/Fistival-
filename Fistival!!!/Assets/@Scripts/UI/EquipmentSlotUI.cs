using Data;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentSlotUI : ItemSlotUI
    {
        private int _idx = 0;

        

        public void SetIDX(int idx)
        {
            _idx = idx;
        }

        public int GetIDX()
        {
            return _idx;
        }
    }
}
