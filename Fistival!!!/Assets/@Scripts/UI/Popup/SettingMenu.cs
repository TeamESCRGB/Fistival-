using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI.Popup
{
    public class SettingMenu : UIPopupBase
    {
        enum Sliders
        {
            VolumeSlider
        }
        enum Texts
        {
            SettingVolumeValueText
        }
        enum Buttons
        {
            SettingExitButton
        }
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Texts));
            Bind<Slider>(typeof(Sliders));
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.SettingExitButton).gameObject.BindUIEvent(OnExit);
            Get<Slider>((int)Sliders.VolumeSlider).onValueChanged.AddListener(OnSliderValueChanged);
            Get<Slider>((int)Sliders.VolumeSlider).value = Managers.Instance.GameManager.MasterVolume;
            
            return true;
        }

        private void OnExit(PointerEventData data)
        {
            Managers.Instance.UIManager.ClosePopupUI();;
        }

        private void OnSliderValueChanged(float value)
        {
            int volume = Mathf.FloorToInt(value * 100);
            GetText((int)Texts.SettingVolumeValueText).text = volume.ToString();
            Managers.Instance.GameManager.MasterVolume = value;
        }

        private void OnDisable()
        {
            Managers.Instance.SaveDataManager.SaveSettings();
        }
    }
}