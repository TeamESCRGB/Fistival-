using Defines;
using Manager;
using Manager.Contents;
using System;
using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class SaveFileMenu : UIPopupBase
    {
        enum Buttons
        {
            SelectButton,
            LeftButton,
            RightButton,
            ExitGameFileMenu
        }

        enum Text
        {
            InfoText,
            PageMaxText,
            PageNowText,
            NowSlotName,
            PlayTime
        }

        enum Images
        {
            Stage1=0,
            Stage2=1,
            Stage3=2,
            Stage4=3,
            Stage5=4,
            Stage6=5,
            Stage7=6
        }

        private SaveFileAccessMode _nowMode = SaveFileAccessMode.LOAD;
        private int _selectedIdx = 0;
        private int _max = 0;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            _max = Managers.Instance.SaveDataManager.GetSelectedSaveSlotCnt();

            if(Managers.Instance.SaveDataManager.IsSaveFileEmpty(0) == false)
            {
                Managers.Instance.SaveDataManager.SelectSaveFile(0);
            }

            BindText(typeof(Text));
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            GetButton((int)Buttons.SelectButton).gameObject.BindUIEvent(OnFileClicked);
            GetButton((int)Buttons.ExitGameFileMenu).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.LeftButton).gameObject.BindUIEvent(OnLeftButton);
            GetButton((int)Buttons.RightButton).gameObject.BindUIEvent(OnRightButton);
            GetText((int)Text.PageMaxText).text = _max.ToString();

            RefreshButtonState();
            RefreshMoveButtonState();

            return true;
        }

        protected override void Start()
        {
            base.Start();
            GetText((int)Text.InfoText).text = _nowMode == SaveFileAccessMode.LOAD ? "LOAD SAVE" : "SAVE";
            RefreshMoveButtonState();
            RefreshButtonState();
        }

        private void OnLeftButton(PointerEventData data)
        {
            if (_selectedIdx > 0)
            {
                _selectedIdx--;
            }

            RefreshButtonState();
            RefreshMoveButtonState();
        }

        private void OnRightButton(PointerEventData data)
        {
            if (_selectedIdx < _max - 1)
            {
                _selectedIdx++;
            }

            RefreshButtonState();
            RefreshMoveButtonState();
        }

        private void RefreshMoveButtonState()
        {
            GetText((int)Text.PageNowText).text = (_selectedIdx + 1).ToString();
            GetButton((int)Buttons.LeftButton).gameObject.SetActive(_selectedIdx > 0);
            GetButton((int)Buttons.RightButton).gameObject.SetActive(_selectedIdx < _max - 1);
        }

        public void SetMenuType(SaveFileAccessMode mode)
        {
            _nowMode = mode;
        }

        public void RefreshButtonState()
        {
            if (_init == false)
            {
                return;
            }
            if (Managers.Instance.SaveDataManager.IsSaveFileEmpty(_selectedIdx))
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "EMPTY";
                UpdateImages(-1);
            }
            else
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "Saved";
                UpdateImages(_selectedIdx);
            }
        }

        private void UpdateImages(int idx)
        {
            GetText((int)Text.NowSlotName).text = $"Slot{_selectedIdx}";

            if (idx < 0)
            {
                GetText((int)Text.PlayTime).text = TimeUtils.SecToTimeStr(0);

                foreach(int e in Enum.GetValues(typeof(Images)))
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>("StageClearDataLocked");
                }

                return;
            }

            var save = Managers.Instance.SaveDataManager.GetSaveFileData();

            GetText((int)Text.PlayTime).text = TimeUtils.SecToTimeStr(save.TotalPlayTime);

            foreach (int e in Enum.GetValues(typeof(Images)))
            {
                if(save.StageSaveDatas.TryGetValue(e,out var value) && value.IsCleared)
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>($"Stage_{e+1}_Cleared");
                }
                else
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>($"StageClearDataLocked");
                }
            }
        }

        private void OnFileClicked(PointerEventData data)
        {
            if (_nowMode == SaveFileAccessMode.LOAD)
            {
                if (Managers.Instance.SaveDataManager.IsSaveFileEmpty(_selectedIdx))
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("save file empty");
                }
                else
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLoadYes, OnConfirmNo);
                }
            }
            else if (_nowMode == SaveFileAccessMode.OVERWRITE)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnOverwriteYes, OnConfirmNo);
            }
        }

        private void OnOverwriteYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            if (Managers.Instance.SaveDataManager.SelectSaveFile(_selectedIdx) == false)
            {
                return;
            }

            var saveData = Managers.Instance.SaveDataManager.GetSaveFileData();
            var clearDict = Managers.Instance.GameManager.GetClearedMapDictRef();
            
            foreach(var val in clearDict)
            {
                if(saveData.StageSaveDatas.ContainsKey(val.Key) == false)
                {
                    saveData.StageSaveDatas[val.Key] = new Data.NonLodable.StageSaveFile();
                }
                saveData.StageSaveDatas[val.Key].IsCleared = val.Value;
            }

            saveData.TotalPlayTime = Managers.Instance.GameManager.GetTotalPlayTime();
            Managers.Instance.SaveDataManager.SaveSaveData();

            RefreshButtonState();
        }

        private void OnLoadYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            if (Managers.Instance.SaveDataManager.SelectSaveFile(_selectedIdx) == false)
            {
                return;
            }

            var nowSceneType = Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType;
            var saveData = Managers.Instance.SaveDataManager.GetSaveFileData();
            var clearDict = Managers.Instance.GameManager.GetClearedMapDictRef();

            clearDict.Clear();
            
            foreach(var val in saveData.StageSaveDatas)
            {
                clearDict[val.Key] = val.Value.IsCleared;
            }

            Managers.Instance.GameManager.InitTotalPlayTimeChecker(saveData.TotalPlayTime);

            if(nowSceneType == SceneType.MainScene)
            {
                Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (_, now, end) =>
                {
                    if (now == end)
                    {
                        Managers.Instance.ResourceManager.ReleaseIn("MainSceneLoaded");
                        Managers.Instance.SceneManagerEx.LoadScene(SceneType.LobbyScene);
                    }
                });
            }
            else if(nowSceneType == SceneType.LobbyScene)
            {
                Debug.Log("로비씬에서 로드함 -- 자리표시자");
            }

        }

        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnExitButton(PointerEventData data)
        {
            if (Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType == SceneType.MainScene)
            {
                GetComponentInParent<BookFlipController>().FlipToFirst();
            }
            else
            {
                Managers.Instance.UIManager.ClosePopupUI();
            }
        }
    }
}