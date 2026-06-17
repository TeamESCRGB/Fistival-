using Data;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
            QueueCard,
            ItemInfoUI
        }


        private ItemData _selectedItem = null;
        private bool _canPause = true;
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
                Get<ShopItemUI>(idx).gameObject.BindUIEvent(OnButtonHover,Defines.UIEventType.POINTER_ENTER);
                Get<ShopItemUI>(idx).gameObject.BindUIEvent(OnButtonExit, Defines.UIEventType.POINTER_EXIT);
            }

            GetButton((int)Buttons.Purchase).gameObject.BindUIEvent(OnPurchase);
            GetButton((int)Buttons.Cancel).gameObject.BindUIEvent(OnCancel);
            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);
            GetText((int)Text.Money).text = $"x {Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.Money}";

            UpdateItemData();

            GetObject((int)Objects.ItemInfoUI).gameObject.SetActive(false);
            GetObject((int)Objects.ItemInfoUI).GetComponent<ItemInfoUI>().Init();

            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
            Managers.Instance.NewInputSystemManager.UI_ESCInput += PauseOpenBind;
            return true;
        }

        private void OnDisable()
        {
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
        }

        private void PauseOpenBind(InputAction.CallbackContext ctx)
        {
            if (ctx.performed == false || _canPause == false)
            {
                return;
            }

            Managers.Instance.GameManager.PauseGame();
            _canPause = false;
        }

        private void LateUpdate()
        {
            if (_canPause)
            {
                return;
            }
            _canPause = Managers.Instance.GameManager.IsGamePaused() == false;
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

        private void OnButtonHover(PointerEventData data)
        {
            if(data.pointerEnter == null || data.pointerEnter.TryGetComponent<ShopItemUI>(out var item) == false)
            {
                GetObject((int)Objects.ItemInfoUI).SetActive(false);
                return;
            }

            var itemData = item.GetData();

            GetObject((int)Objects.ItemInfoUI).SetActive(true);
            GetObject((int)Objects.ItemInfoUI).GetComponent<ItemInfoUI>().SetName(itemData.Name).SetDescription(itemData.Description);

            var infoRect = GetObject((int)Objects.ItemInfoUI).GetComponent<RectTransform>();
            var buttonRect = data.pointerEnter.GetComponent<RectTransform>();

            Canvas canvas = GetComponentInParent<Canvas>();
            float scale = canvas.scaleFactor;
            Vector3 pos = data.pointerEnter.transform.position;

            if (pos.y > Screen.height/2)
            {
                pos.y -= (infoRect.rect.height + buttonRect.rect.height) * scale;
            }

            if (pos.x < Screen.width / 2)
            {
                pos.x += (infoRect.rect.width) * scale;
            }

            infoRect.position = pos;
        }

        private void OnButtonExit(PointerEventData data)
        {
            GetObject((int)Objects.ItemInfoUI).SetActive(false);
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