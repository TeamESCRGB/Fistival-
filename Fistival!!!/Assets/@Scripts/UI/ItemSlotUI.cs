using Data;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemSlotUI : MonoBehaviour
    {
        protected Image _itemImage;
        protected EquipmentData _data;

        private void Awake()
        {
            _itemImage = transform.parent.GetComponent<Image>();
        }


        public virtual void SetItem(EquipmentData data)
        {
            if (data is null || Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.PurchasedItems.Contains(data.Idx) == false)
            {
                _data = null;
                transform.parent.gameObject.SetActive(false);
                return;
            }
            _itemImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);
            _data = data;
        }

        public EquipmentData GetData()
        {
            return _data;
        }
    }
}
