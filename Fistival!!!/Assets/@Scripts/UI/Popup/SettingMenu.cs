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
            VolumeSlider,
            BGMVolumeSlider,
            SFXVolumeSlider
        }
        enum Texts
        {
            SettingVolumeValueText,
            SettingBGMValueText,
            SettingSFXValueText
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

            Get<Slider>((int)Sliders.VolumeSlider).onValueChanged.AddListener(OnMasterVolume);
            Get<Slider>((int)Sliders.VolumeSlider).value = Managers.Instance.GameManager.MasterVolume;

            Get<Slider>((int)Sliders.BGMVolumeSlider).onValueChanged.AddListener(OnBGMVolume);
            Get<Slider>((int)Sliders.BGMVolumeSlider).value = Managers.Instance.GameManager.BGMVolume;

            Get<Slider>((int)Sliders.SFXVolumeSlider).onValueChanged.AddListener(OnSFXVolume);
            Get<Slider>((int)Sliders.SFXVolumeSlider).value = Managers.Instance.GameManager.SFXVolume;

            return true;
        }

        private void OnExit(PointerEventData data)
        {
            Managers.Instance.UIManager.ClosePopupUI();;
        }

        private void OnSliderValueChanged(float value, int idx)
        {
            int volume = Mathf.FloorToInt(value * 100);
            GetText(idx).text = volume.ToString();
        }

        private void OnMasterVolume(float value)
        {
            Managers.Instance.GameManager.MasterVolume = value;
            OnSliderValueChanged(value, (int)Texts.SettingVolumeValueText);
        }

        private void OnBGMVolume(float value)
        {
            Managers.Instance.GameManager.BGMVolume = value;
            OnSliderValueChanged(value, (int)Texts.SettingBGMValueText);
        }

        private void OnSFXVolume(float value)
        {
            Managers.Instance.GameManager.SFXVolume = value;
            OnSliderValueChanged(value, (int)Texts.SettingSFXValueText);
        }

        private void OnDisable()
        {
            Managers.Instance.SaveDataManager.SaveSettings();
        }
    }
}