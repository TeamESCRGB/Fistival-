using Coordinator;
using Data;
using Defines;
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

        private void SetupItemButton(ItemSlotUI ui,EquipmentData item, Action<PointerEventData> clickCallback)
        {
            ui.SetItem(item);
            ui.gameObject.BindUIEvent(clickCallback);
            ui.gameObject.BindUIEvent(OnHover,Defines.UIEventType.POINTER_ENTER);
            ui.gameObject.BindUIEvent(OnHoverExit,Defines.UIEventType.POINTER_EXIT);
        }

        private void SetupEquipmentButton(EquipmentSlotUI slot, int itemIdx)
        {
            EquipmentData item;
            if(Managers.Instance.DataManager.EquipmentDataDict.TryGetValue(itemIdx, out item) == false)
            {
                item = null;
            }
            SetupItemButton(slot,item,OnEquipmentSlotClicked);
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
            SetupEquipmentButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem1), save.EquippedItems[0]);
            SetupEquipmentButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem2), save.EquippedItems[1]);
            SetupEquipmentButton(Get<EquipmentSlotUI>((int)Equipments.EquippedItem3), save.EquippedItems[2]);

            Get<EquipmentSlotUI>((int)Equipments.EquippedItem1).SetIDX(0);
            Get<EquipmentSlotUI>((int)Equipments.EquippedItem2).SetIDX(1);
            Get<EquipmentSlotUI>((int)Equipments.EquippedItem3).SetIDX(2);

            foreach (var data in Managers.Instance.DataManager.EquipmentDataDict.Values)
            {
                SetupItemButton(Get<ItemSlotUI>(data.Idx), data, OnItemClicked);
            }

            return true;
        }

        private void OnEquipmentSlotClicked(PointerEventData data)
        {
            if(data.pointerClick.TryGetComponent<EquipmentSlotUI>(out var comp) == false || comp.GetData() is null)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "UnequipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                return;
            }

            if(_player != null)
            {
                _player.EquipItem(comp.GetIDX(), -1);
            }

            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, comp.GetData().UnEquipSound, false, Managers.Instance.GameManager.SFXVolume);

            comp.SetItem(null);
            GetObject((int)Objects.ItemInfoUI).SetActive(false);
        }

        private void OnItemClicked(PointerEventData data)
        {
            if (data.pointerClick.TryGetComponent<ItemSlotUI>(out var comp) == false || comp.GetData() is null)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "EquipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                return;
            }




            var item = comp.GetData();
            int slotIdx = -1;
            int emptyCnt = 0;
            bool isEquipmentLocked = false;

            for(int i = 2; i >= 0; i--)
            {
                var slot = Get<EquipmentSlotUI>(i);
                
                if(slotIdx < 0)
                {
                    if(slot.GetData() is null)
                    {
                        slotIdx = i;
                    }
                }

                if(slot.GetData() is null)
                {
                    emptyCnt++;
                }
                else if(isEquipmentLocked == false)
                {
                    isEquipmentLocked = slot.GetData().Idx == 10;
                }

                if (slot.GetData()?.Idx == item.Idx)
                {
                    Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "EquipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                    return;
                }

            }

            if(slotIdx < 0)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "EquipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                return;
            }

            if(item.Idx == 10 && emptyCnt < 3)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "EquipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("모든 것을 가지려면 모든 것을 잃어야 하는 법. 모든 착용품을 해제해라.");
                return;
            }
            else if(isEquipmentLocked)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "EquipFailedSFX", false, Managers.Instance.GameManager.SFXVolume);
                Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("마지막처럼은 혼자서만 쓸 수 있다. 이걸 빼던지 다른걸 포기하던지 선택해라.");
                return;
            }

            if (_player != null)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, item.EquipSound, false, Managers.Instance.GameManager.SFXVolume);
                _player.EquipItem(slotIdx, item.Idx);
                Get<EquipmentSlotUI>(slotIdx).SetItem(item);
            }
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
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", true, Managers.Instance.GameManager.SFXVolume);
        }
    }
}