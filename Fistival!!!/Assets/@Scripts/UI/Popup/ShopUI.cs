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
            SelectedItemPrice,
            Money
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
                if (Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.PurchasedItems.Contains(idx))
                {
                    Get<ShopItemUI>(idx).SetPurchased();
                }
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
            GetText((int)Text.Money).text = $"x {Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.Money}";

            UpdateItemData();

            return true;
        }

        private void UpdateItemData()
        {
            if(_selectedItem is null)
            {
                GetObject((int)Objects.QueueCard).SetActive(false);Debug.Log("null");
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
            var data = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData;
            if(data.Money < _selectedItem.Price)
            {
                return;
            }
            data.Money -= _selectedItem.Price;
            data.PurchasedItems.Add(_selectedItem.Idx);
            Get<ShopItemUI>(_selectedItem.Idx).SetPurchased();
            _selectedItem = null;
            GetText((int)Text.Money).text = $"x {data.Money}";
            UpdateItemData();
        }

        private void OnCancel(PointerEventData _)
        {
            _selectedItem = null;
            UpdateItemData();
        }
    }
}