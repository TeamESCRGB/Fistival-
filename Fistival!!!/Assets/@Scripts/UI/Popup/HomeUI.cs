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
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.ExitButton).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.EquipmentButton).gameObject.BindUIEvent(OnEquipmentButton);
            GetButton((int)Buttons.DataButton).gameObject.BindUIEvent(OnDataButton);
            GetButton((int)Buttons.SaveButton).gameObject.BindUIEvent(OnSaveButton);


            var clearedMapDict = Managers.Instance.GameManager.GetClearedMapDictRef();
            /*
             캐릭터 업데이트하는 코드 작성 필요
            이미지는 StageData의 HomeIMG보여주면 되는데, 그걸 어떻게 해야되나
             */


            return true;
        }

        private void OnEquipmentButton(PointerEventData _)
        {

        }
        private void OnSaveButton(PointerEventData _)
        {

        }
        private void OnDataButton(PointerEventData _)
        {

        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}