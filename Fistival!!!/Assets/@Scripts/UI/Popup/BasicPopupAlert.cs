using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class BasicPopupAlert : UIPopupBase
    {
        enum Buttons
        {
            Yes
        }

        enum Text
        {
            BasicPopupAlertText
        }

        private string _text="";

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Text));
            BindButton(typeof(Buttons));
            GetButton((int)Buttons.Yes).gameObject.BindUIEvent(OnYes);
            GetText((int)Text.BasicPopupAlertText).text = _text;
            return true;
        }
        public void SetText(string text)
        {
            _text = text;
        }

        private void OnYes(PointerEventData data)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}
