using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI.Scene
{
    public class UI_TitleScene : UISceneBase
    {
        enum Buttons
        {
            StartButton
        }

        enum Objects
        {
            LoadProgressBar
        }

        enum Texts
        {
            LoadingLabelName,
            LoadingAssetName,
            VersionText
        }
        [SerializeField]
        private int _loadedCnt = 0;

        private bool _isOpeningEnd = false;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));
            BindText(typeof(Texts));

            GetButton((int)Buttons.StartButton).gameObject.BindUIEvent(OnStartButtonPressed);
            GetButton((int)Buttons.StartButton).gameObject.SetActive(false);

            GetText((int)Texts.LoadingLabelName).text = "StaticLoaded";
            GetText((int)Texts.LoadingAssetName).text = "";
            GetObject((int)Objects.LoadProgressBar).GetComponent<Slider>().value = 0;
            GetText((int)Texts.VersionText).text = $"v{Application.version}";

            var setting = Managers.Instance.SaveDataManager.GetGameSettingRef();

            Managers.Instance.GameManager.MasterVolume = setting.MasterVolume;
            Managers.Instance.GameManager.BGMVolume = setting.BGMVolume;
            Managers.Instance.GameManager.SFXVolume = setting.SFXVolume;

            Managers.Instance.ResourceManager.LoadAsyncAllIn("StaticLoaded", (asset, now, end) =>
            {
                LoadAssets(asset, now, end);
                if(now==end)
                {
                    GetText((int)Texts.LoadingLabelName).text = "TitleSceneLoaded";
                    GetText((int)Texts.LoadingAssetName).text = "";
                    GetObject((int)Objects.LoadProgressBar).GetComponent<Slider>().value = 0;
                    Managers.Instance.ResourceManager.LoadAsyncAllIn("TitleSceneLoaded", LoadAssets);
                }
            });

            return true;
        }

        private void LoadAssets(string assetName, int now, int end)
        {
            if(end == 0)
            {
                now = end = 1;
            }

            GetText((int)Texts.LoadingAssetName).text = assetName;
            GetObject((int)Objects.LoadProgressBar).GetComponent<Slider>().value = (float)now / end;

            if (now == end)
            {
                _loadedCnt++;
                GetText((int)Texts.LoadingAssetName).text = "load complete";
                if(_loadedCnt == 2)
                {
                    InitAfterLoad();
                }
            }
            
        }

        private void InitAfterLoad()
        {
            Managers.Instance.DataManager.Init();
            Managers.Instance.GlobalSoundManager.Init();
            Managers.Instance.UIManager.Init();
            GetButton((int)Buttons.StartButton).gameObject.SetActive(true);
            GetButton((int)Buttons.StartButton).GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic).Play();
            GetObject((int)Objects.LoadProgressBar).SetActive(false);
            GetText((int)Texts.LoadingLabelName).gameObject.SetActive(false);
            GetText((int)Texts.LoadingAssetName).gameObject.SetActive(false);
            AudioListener.volume = Managers.Instance.GameManager.MasterVolume;
        }

        public void OnOpeningEnd()
        {
            _isOpeningEnd  = true;
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.BGM_0, "MainSceneBGM", true, Managers.Instance.GameManager.BGMVolume);
        }

        private void OnStartButtonPressed(PointerEventData data)
        {
            if (_loadedCnt < 2 || _isOpeningEnd == false)
            {
                return;
            }


            Managers.Instance.ResourceManager.LoadAsyncAllIn("MainSceneLoaded", (_, now, end) =>
            {
                if (now < end)
                {
                    return;
                }
                Managers.Instance.SceneManagerEx.LoadScene(Defines.SceneType.MainScene);
                Managers.Instance.ResourceManager.ReleaseIn("TitleSceneLoaded");

            });

        }
    }
}