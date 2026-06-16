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

            var save = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData;

            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem1), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[0]], OnEquipmentSlotClicked);
            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem2), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[1]], OnEquipmentSlotClicked);
            SetupItemButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem3), Managers.Instance.DataManager.ItemDataDIct[save.EquippedItems[2]], OnEquipmentSlotClicked);
            
            foreach(var data in Managers.Instance.DataManager.ItemDataDIct.Values)
            {
                SetupItemButton(Get<ItemSlotUI>(data.Idx), data, OnItemClicked);
            }

            return true;
        }

        private void OnEquipmentSlotClicked(PointerEventData data)
        {
            Debug.Log(data.pointerClick.GetComponent<ItemSlotUI>().GetData().Name);
        }

        private void OnItemClicked(PointerEventData data)
        {
            Debug.Log(data.pointerClick.GetComponent<ItemSlotUI>().GetData().Name);
        }

        private void OnHover(PointerEventData data)
        {

        }

        private void OnHoverExit(PointerEventData data)
        {

        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}