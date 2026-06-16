using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class HomeUI : UIPopupBase
    {
        enum Buttons
        {
            EquipmentButton,
            DataButton,
            SaveButton,
            ExitButton
        }
        enum Images
        {
            Stage1=0,
            Stage2=1,
            Stage3=2,
            Stage4=3,
            Stage5=4,
            Stage6=5,
            Stage7=6
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));

            GetButton((int)Buttons.ExitButton).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.EquipmentButton).gameObject.BindUIEvent(OnEquipmentButton);
            GetButton((int)Buttons.DataButton).gameObject.BindUIEvent(OnDataButton);
            GetButton((int)Buttons.SaveButton).gameObject.BindUIEvent(OnSaveButton);


            foreach(var data in Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas)
            {
                if(data.Value.IsCleared)
                {
                    //이거 이미지 로드할까 생각해봤는데, 그냥 직접 배치해두고 껐다켰다하는게 더 좋을듯
                    GetImage(data.Key).gameObject.SetActive(true);
                }
                else
                {
                    GetImage(data.Key).gameObject.SetActive(false);
                }
            }

            return true;
        }

        private void OnEquipmentButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<EquipmentUI>("EquipmentUI");
        }
        private void OnSaveButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(Defines.SaveFileAccessMode.OVERWRITE);
        }
        private void OnDataButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<DataUI>("DataUI");
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}