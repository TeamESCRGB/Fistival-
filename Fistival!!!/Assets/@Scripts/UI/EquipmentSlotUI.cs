using Data;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentSlotUI : ItemSlotUI
    {
        private int _idx = 0;

        public override void SetItem(EquipmentData data)
        {
            if (data is null)
            {
                _data = null;
                _itemImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>("NullImage");
                return;
            }

            _itemImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);

            _data = data;
        }

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
