using Defines;
using Manager;
using TMPro;
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
            PageNowText
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
            GetButton((int)Buttons.SelectButton).gameObject.BindUIEvent(OnFileClicked);
            GetButton((int)Buttons.ExitGameFileMenu).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.LeftButton).gameObject.BindUIEvent(OnLeftButton);
            GetButton((int)Buttons.RightButton).gameObject.BindUIEvent(OnRightButton);
            GetText((int)Text.PageMaxText).text = _max.ToString();

            RefreshButtonState();
            RefreshMoveButtonState();

            return true;
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
                //여기에 이미지 보여주고 그런 로직 추가하기
            }
            else
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "Saved";
                //여기에 이미지 보여주고 그런 로직 추가하기
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
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}
