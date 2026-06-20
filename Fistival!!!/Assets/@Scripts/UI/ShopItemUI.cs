using Data;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopItemUI : MonoBehaviour
    {
        private Image _itemImage;
        private TMP_Text _priceText;
        private EquipmentData _data;
        private GameObject _purchased;
        private bool _isPurchased;

        private void Awake()
        {
            _purchased = transform.Find("PurchasedPanel").gameObject;
            _itemImage = GetComponent<Image>();
            _priceText = GetComponentInChildren<TMP_Text>();
        }

        public void SetPurchased()
        {
            _isPurchased = true;
            _purchased.SetActive(true);
            transform.Find("Price").gameObject.SetActive(false);
        }

        public void SetItem(EquipmentData data)
        {
            if(data is null)
            {
                _data = null;
                gameObject.SetActive(false);
                return;
            }
            _data = data;
            _itemImage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);
            _priceText.text = $"x {data.Price}";
        }

        public EquipmentData GetData()
        {
            if(_isPurchased)
            {
                return null;
            }
            return _data;
        }
    }
}