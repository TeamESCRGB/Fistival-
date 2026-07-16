using Data;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using DG.Tweening;

namespace UI.Popup
{
    public class TalkCutScenePopup : UIPopupBase
    {
        private TalkScriptData _data;
        private int _idx;

        enum Images
        {
            CharacterSprite
        }

        enum Texts
        {
            Name,
            Script
        }

        enum Buttons
        {
            Next
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.Next).gameObject.BindUIEvent(OnNextButton);
            
            return true;
        }

        public void SetData(string dataKey)
        {
            Managers.Instance.DataManager.TalkScriptDataDict.TryGetValue(dataKey, out _data);
        }

        private void OnNextButton(PointerEventData _)
        {

        }
    }
}