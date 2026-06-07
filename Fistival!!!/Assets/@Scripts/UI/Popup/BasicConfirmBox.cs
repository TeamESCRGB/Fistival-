using Manager;
using System;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class BasicConfirmBox : UIPopupBase
    {
        enum Buttons
        {
            Yes,
            No
        }

        private Action _onYes;
        private Action _onNo;


        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }


            BindButton(typeof(Buttons));
            GetButton((int)Buttons.Yes).gameObject.BindUIEvent(OnYes);
            GetButton((int)Buttons.No).gameObject.BindUIEvent(OnNo);
            

            return true;
        }

        public void SetCallback(Action onYes, Action onNo)
        {
            _onYes = onYes;
            _onNo = onNo;
        }

        private void OnYes(PointerEventData data)
        {
            _onYes?.Invoke();
        }

        private void OnNo(PointerEventData data)
        {
            _onNo?.Invoke();
        }

        private void OnDisable()
        {
            _onYes = null;
            _onNo = null;
        }
    }
}