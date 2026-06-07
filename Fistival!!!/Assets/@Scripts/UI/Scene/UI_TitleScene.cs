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
            LoadingAssetName
        }
        [SerializeField]
        private int _loadedCnt = 0;

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
            AudioListener.volume = Managers.Instance.SaveDataManager.GetGameSettingRef().Volume;
        }


        private void OnStartButtonPressed(PointerEventData data)
        {
            if (_loadedCnt < 2)
            {
                return;
            }


            Managers.Instance.ResourceManager.LoadAsyncAllIn("MainScene", (_, now, end) =>
            {
                if (now < end)
                {
                    return;
                }
                Managers.Instance.SceneManagerEx.LoadScene(Defines.SceneType.MainScene);
                Managers.Instance.ResourceManager.ReleaseIn("TitleScene");

            });

        }
    }
}