using Data;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class ShopUI : UIPopupBase
    {
        enum Items
        {
            Item1,
            Item2,
            Item3,
            Item4,
            Item5,
            Item6,
            Item7,
            Item8,
            Item9,
            Item10,
            Item11,
            Item12
        }

        enum Buttons
        {
            Exit,
            Purchase,
            Cancel
        }

        enum Images
        {
            ItemPreview
        }

        enum Text
        {
            SelectedItemPrice
        }

        enum Objects
        {
            QueueCard
        }

        private ItemData _selectedItem = null;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindImage(typeof(Images));
            Bind<ShopItemUI>(typeof(Items));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));
            BindText(typeof(Text));
            GetObject((int)Objects.QueueCard).SetActive(false);


            foreach(int idx in System.Enum.GetValues(typeof(Items)))
            {
                Get<ShopItemUI>(idx).gameObject.SetActive(true);
                Get<ShopItemUI>(idx).gameObject.BindUIEvent(OnButton);
                ItemData item = null;
                if(Managers.Instance.DataManager.ItemDataDIct.TryGetValue(idx, out item) == false)
                {
                    item = null;
                }
                Get<ShopItemUI>(idx).SetItem(item);
            }

            GetButton((int)Buttons.Purchase).gameObject.BindUIEvent(OnPurchase);
            GetButton((int)Buttons.Cancel).gameObject.BindUIEvent(OnCancel);
            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);

            UpdateItemData();

            return true;
        }

        private void UpdateItemData()
        {
            if(_selectedItem is null)
            {
                GetObject((int)Objects.QueueCard).SetActive(false);
                return;
            }
            GetObject((int)Objects.QueueCard).SetActive(true);
            GetImage((int)Images.ItemPreview).sprite = Managers.Instance.ResourceManager.Load<Sprite>(_selectedItem.Image);
            GetText((int)Text.SelectedItemPrice).text = $"x {_selectedItem.Price}";
        }

        private void OnButton(PointerEventData data)
        {
            _selectedItem = data.pointerClick.GetComponent<ShopItemUI>().GetData();
            UpdateItemData();
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnPurchase(PointerEventData _)
        {

        }

        private void OnCancel(PointerEventData _)
        {
            _selectedItem = null;
            UpdateItemData();
        }
    }
}