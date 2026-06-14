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
        private ItemData _data;


        private void Awake()
        {
            _itemImage = GetComponent<Image>();
            _priceText = GetComponentInChildren<TMP_Text>();
        }

        public void SetItem(ItemData data)
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

        public ItemData GetData()
        {
            return _data;
        }
    }
}