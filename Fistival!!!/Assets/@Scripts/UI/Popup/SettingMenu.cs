using Defines;
using Manager;
using UI.Transition;
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
        enum Images
        {
            MasterVolumeImg,
            BGMVolumeImg,
            SFXVolumeImg
        }

        private const float _section1Max = 0.33333f;
        private const float _section2Max = 0.66666f;

        private int _masterLastSection;
        private int _bgmLastSection;
        private int _sfxLastSection;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Texts));
            Bind<Slider>(typeof(Sliders));
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));

            _masterLastSection = -1;
            _bgmLastSection = -1;
            _sfxLastSection = -1;

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
            if (Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType == SceneType.MainScene)
            {
                GetComponentInParent<BookFlipController>().FlipToFirst();
            }
            else
            {
                Managers.Instance.UIManager.ClosePopupUI();
            }
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false);
        }

        private bool LoadImg(float volume, string section1, string section2, string section3, out Sprite img, ref int section)
        {
            img = null;

            if(volume <= _section1Max)
            {
                if(section != 1)
                {
                    img = Managers.Instance.ResourceManager.Load<Sprite>(section1);
                    section = 1;
                    return true;
                }
            }
            else if (volume <= _section2Max)
            {
                if(section != 2)
                {
                    img = Managers.Instance.ResourceManager.Load<Sprite>(section2);
                    section = 2;
                    return true;
                }
            }
            else
            {
                if(section != 3)
                {
                    img = Managers.Instance.ResourceManager.Load<Sprite>(section3);
                    section = 3;
                    return true;
                }
            }
            return false;
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
            if(LoadImg(value, "MasterVolumeSection1", "MasterVolumeSection2", "MasterVolumeSection3",out var img, ref _masterLastSection))
            {
                GetImage((int)Images.MasterVolumeImg).sprite = img;
            }
        }

        private void OnBGMVolume(float value)
        {
            Managers.Instance.GameManager.BGMVolume = value;
            OnSliderValueChanged(value, (int)Texts.SettingBGMValueText);
            if(LoadImg(value, "BGMVolumeSection1", "BGMVolumeSection2", "BGMVolumeSection3", out var img, ref _bgmLastSection))
            {
                GetImage((int)Images.BGMVolumeImg).sprite = img;
            }
        }

        private void OnSFXVolume(float value)
        {
            Managers.Instance.GameManager.SFXVolume = value;
            OnSliderValueChanged(value, (int)Texts.SettingSFXValueText);
            if(LoadImg(value, "SFXVolumeSection1", "SFXVolumeSection2", "SFXVolumeSection3", out var img, ref _sfxLastSection))
            {
                GetImage((int)Images.SFXVolumeImg).sprite = img;
            }
        }

        private void OnDisable()
        {
            Managers.Instance.SaveDataManager.SaveSettings();
        }
    }
}