using Coordinator;
using Data;
using Manager;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class EquipmentUI : UIPopupBase
    {
        enum Buttons
        {
            ExitButton
        }

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

        enum Equipments
        {
            EquippedItem1,
            EquippedItem2,
            EquippedItem3,
        }


        enum Objects
        {
            ItemInfoUI
        }

        private PlayerCoordinator _player;

        private void SetupItemButton(ItemSlotUI ui,ItemData item, Action<PointerEventData> clickCallback)
        {
            ui.SetItem(item);
            ui.gameObject.BindUIEvent(clickCallback);
            ui.gameObject.BindUIEvent(OnHover,Defines.UIEventType.POINTER_ENTER);
            ui.gameObject.BindUIEvent(OnHoverExit,Defines.UIEventType.POINTER_EXIT);
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            BindObject(typeof(Objects));
            BindButton(typeof(Buttons));
            Bind<ItemSlotUI>(typeof(Items));
            Bind<EquipmentSlotUI>(typeof(Equipments));

            GetObject((int)Objects.ItemInfoUI).gameObject.SetActive(false);
            GetObject((int)Objects.ItemInfoUI).GetComponent<ItemInfoUI>().Init();
            GetButton((int)Buttons.ExitButton).gameObject.BindUIEvent(OnExitButton);

            _player = FindAnyObjectByType<PlayerCoordinator>();

            var save = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData;

            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem1), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[0]], OnEquipmentSlotClicked);
            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem2), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[1]], OnEquipmentSlotClicked);
            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem3), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[2]], OnEquipmentSlotClicked);

            Get<EquipmentSlotUI>((int)Equipments.EquippedItem1).SetIDX(0);
            Get<EquipmentSlotUI>((int)Equipments.EquippedItem2).SetIDX(1);
            Get<EquipmentSlotUI>((int)Equipments.EquippedItem3).SetIDX(2);

            foreach (var data in Managers.Instance.DataManager.ItemDataDIct.Values)
            {
                SetupItemButton(Get<ItemSlotUI>(data.Idx), data, OnItemClicked);
            }

            return true;
        }

        private void OnEquipmentSlotClicked(PointerEventData data)
        {
            if(data.pointerClick.TryGetComponent<EquipmentSlotUI>(out var comp) == false || comp.GetData() is null)
            {
                Debug.Log("emp");
                return;
            }

            if(_player != null)
            {
                _player.EquipItem(comp.GetIDX(), -1);
            }
            
            comp.SetItem(null);
            GetObject((int)Objects.ItemInfoUI).SetActive(false);
        }

        private void OnItemClicked(PointerEventData data)
        {
            Debug.Log(data.pointerClick.GetComponent<ItemSlotUI>().GetData().Name);
        }

        private void OnHover(PointerEventData data)
        {
            if (data.pointerEnter == null || data.pointerEnter.TryGetComponent<ItemSlotUI>(out var item) == false || item.GetData() is null)
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

            if (pos.y > Screen.height / 2)
            {
                pos.y -= (infoRect.rect.height + buttonRect.rect.height) * scale;
            }

            if (pos.x < Screen.width / 2)
            {
                pos.x += (buttonRect.rect.width) * scale;
            }

            infoRect.position = pos;
        }

        private void OnHoverExit(PointerEventData data)
        {
            GetObject((int)Objects.ItemInfoUI).SetActive(false);
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}