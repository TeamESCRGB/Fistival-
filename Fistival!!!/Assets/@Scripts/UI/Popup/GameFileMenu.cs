using Defines;
using Manager;
using System.Collections.Generic;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class GameFileMenu : UIPopupBase
    {
        enum Buttons
        {
            GameSlotButton1 = 0,
            GameSlotButton2 = 1,
            GameSlotButton3 = 2,
            GameSlotButton4 = 3,
            GameSlotButton5 = 4,
            GameSlotButton6 = 5
        }


        private Dictionary<GameObject, int> _slotToIdxConverter = new Dictionary<GameObject, int>();
        private SaveFileAccessMode _nowMode = SaveFileAccessMode.LOAD;
        private int _selectedIdx = 0;
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            RefreshButtonState();

            return true;
        }
        
        public void RefreshButtonState()
        {
            if(_init == false)
            {
                return;
            }
            for (int i = 0; i < 6; i++)
            {
                var buttonGameObject = GetButton(i).gameObject;
                _slotToIdxConverter[buttonGameObject] = i;
                buttonGameObject.BindUIEvent(OnFileClicked);

                if (Managers.Instance.SaveDataManager.IsGameFileEmpty(i))
                {
                    buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "EMPTY";
                }
                else
                {
                    buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Saved";
                }

            }
        }

        private void OnFileClicked(PointerEventData data)
        {
            if(_slotToIdxConverter.TryGetValue(data.pointerClick,out _selectedIdx) == false)
            {
                return;
            }

            if(_nowMode == SaveFileAccessMode.LOAD)
            {
                if(Managers.Instance.SaveDataManager.IsGameFileEmpty(_selectedIdx))
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("game file empty");
                }
                else
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLoadYes, OnConfirmNo);
                }
            }
            else if(_nowMode == SaveFileAccessMode.OVERWRITE)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnOverwriteYes, OnConfirmNo);
            }
        }

        private void OnOverwriteYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            

        }

        private void OnLoadYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

    }
}