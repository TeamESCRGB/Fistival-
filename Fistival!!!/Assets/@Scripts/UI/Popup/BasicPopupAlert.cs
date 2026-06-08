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

        private string _text;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Text));
            BindButton(typeof(Buttons));
            GetButton((int)Buttons.Yes).gameObject.BindUIEvent(OnYes);
            Debug.Log(_bindedObjects[typeof(TMP_Text)] == null);
            return true;
        }

        protected override void Start()
        {
            base.Start();
            GetText((int)Text.BasicPopupAlertText).text = _text;
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
