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

        enum Texts
        {
            Content
        }

        private Action _onYes;
        private Action _onNo;
        private string _text="";

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            GetButton((int)Buttons.Yes).gameObject.BindUIEvent(OnYes);
            GetButton((int)Buttons.No).gameObject.BindUIEvent(OnNo);
            GetText((int)Texts.Content).text = _text;

            return true;
        }

        public BasicConfirmBox SetCallback(Action onYes, Action onNo)
        {
            _onYes = onYes;
            _onNo = onNo;
            return this;
        }

        public BasicConfirmBox SetText(string text)
        {
            _text = text;
            return this;
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