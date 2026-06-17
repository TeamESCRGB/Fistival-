using Defines;
using Manager;
using System;
using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class GameFileMenu : UIPopupBase
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
            Stage1 = 0,
            Stage2 = 1,
            Stage3 = 2,
            Stage4 = 3,
            Stage5 = 4,
            Stage6 = 5,
            Stage7 = 6
        }

        private int _selectedIdx = 0;
        private int _max = 0;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }


            _max = Managers.Instance.SaveDataManager.GetSelectedSaveSlotCnt();
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

        public void Open()
        {
            bool skip = true;

            for (int i = 0; i < _max; i++)
            {
                if (Managers.Instance.SaveDataManager.IsSaveFileEmpty(i) == false)
                {
                    skip = false;
                    break;
                }
            }

            if (skip)
            {
                OnOverwriteYes();
            }
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
                Managers.Instance.SaveDataManager.SelectSaveFile(_selectedIdx);
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                UpdateImages(_selectedIdx);
            }
        }

        private void UpdateImages(int idx)
        {
            GetText((int)Text.NowSlotName).text = $"SaveSlot #{_selectedIdx+1}";

            if(idx < 0)
            {
                GetText((int)Text.PlayTime).text = TimeUtils.SecToTimeStr(0);

                foreach (int e in Enum.GetValues(typeof(Images)))
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>("StageClearDataLocked");
                }

                return;
            }

            var save = Managers.Instance.SaveDataManager.GetSaveFile(_selectedIdx);//이거는 이 함수 호출부에서 파일이 있는지 검사하기에 문제x

            GetText((int)Text.PlayTime).text = TimeUtils.SecToTimeStr(save.TotalPlayTime);

            foreach (int e in Enum.GetValues(typeof(Images)))
            {
                if (save.StageSaveDatas.TryGetValue(e, out var value) && value.IsCleared)
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>($"Stage_{e + 1}_Cleared");
                }
                else
                {
                    GetImage(e).sprite = Managers.Instance.ResourceManager.Load<Sprite>($"StageClearDataLocked");
                }
            }
        }

        private void OnFileClicked(PointerEventData data)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnOverwriteYes, OnConfirmNo);
        }

        private void OnOverwriteYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.GameManager.ClearClearedMapDict();

            Managers.Instance.SaveDataManager.ClearAllSaveFile();

            Managers.Instance.SaveDataManager.SelectSaveFile(0);

            Managers.Instance.SaveDataManager.SaveSaveData();

            Managers.Instance.GameManager.InitTotalPlayTimeChecker(0);

            Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (_, now, end) =>
            {
                if (now == end)
                {
                    Managers.Instance.ResourceManager.ReleaseIn("MainSceneLoaded");
                    Managers.Instance.SceneManagerEx.LoadScene(SceneType.LobbyScene);
                    //여기에 컷씬 보여주고 그런거 추가
                }
            });
        }
        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnExitButton(PointerEventData data)
        {
            if(Managers.Instance.SceneManagerEx.CurrentScene.NowSceneType == SceneType.MainScene)
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
