using Defines;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class DataUI : UIPopupBase
    {
        enum Buttons
        {
            ExitButton
        }

        enum Texts
        {
            TotalPlayTime,
            Money,
            SaveIdx
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            GetText((int)Texts.Money).text = Managers.Instance.SaveDataManager.GetSaveFileData().PlayerSaveData.Money.ToString();
            GetText((int)Texts.SaveIdx).text = $"Save #{Managers.Instance.SaveDataManager.GetSelectedSaveIDX() + 1}";


            GetButton((int)Buttons.ExitButton).gameObject.BindUIEvent(OnExitButton);

            return true;
        }

        private void Update()
        {
            GetText((int)Texts.TotalPlayTime).text = Utils.TimeUtils.SecToTimeStr(Managers.Instance.GameManager.GetTotalPlayTime());
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false);
        }
    }
}