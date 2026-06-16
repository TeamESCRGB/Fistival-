using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentSlotUI : ItemSlotUI
    {

        public override void SetItem(ItemData data)
        {
            if (data is null)
            {
                _data = null;
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            
            _data = data;
        }
    }
}
